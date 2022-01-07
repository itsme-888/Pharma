using Pharma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Pharma.Sql;
using Pharma.Attributes;

namespace Pharma
{
    class Program
    {
        static readonly Type[] allTables = { 
            typeof(Pharmacy), 
            typeof(Product), 
            typeof(StoreHouse), 
            typeof(Part) };

        static readonly Type PharmaTableType = allTables[0];

        #region ConsoleOutput
        static void ShowUserMessage()
        {
            Console.WriteLine("Выберите команду: ");
            Console.WriteLine();
            Console.WriteLine("Выбор таблицы для добавления или удаления данных: ");

            for(int i = 0; i < allTables.Length; i++)
            {
                TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(allTables[i], typeof(TableAttribute));
                Console.WriteLine($"{i + 1} - Таблица '{tableAttribute?.Desc ?? allTables[i].Name}'");
            }

            Console.WriteLine();
            Console.WriteLine($"{allTables.Length + 1} - Отобразить весь список товаров и его количество в выбранной аптеке (количество товара во всех складах аптеки)");
            Console.WriteLine("0 - Выход");
        }

        static void ShowUserMessage(Type type)
        {
            if (type == null)
            {
                ShowUserMessage();
            }
            else
            {
                TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute));

                Console.WriteLine($"Что необходимо сделать с таблицей '{tableAttribute?.Desc ?? type.Name}'?");
                Console.WriteLine("1 - Добавить запись");
                Console.WriteLine("2 - Удалить запись");
                Console.WriteLine("0 - Отмена");
            }
        }

        static void ShowErrorCommandMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неверная команда!");
            Console.WriteLine();
            Console.ResetColor();
        }
        private static void ShowData(List<string[]> data)
        {
            foreach (var d in data)
            {
                Console.WriteLine(string.Join("\t|\t", d));
            }
        }

        #endregion

        #region ConsoleDataHelpers
        private static string[] AvailableIds(List<string[]> data)
        {
            var titles = data.First();
            int idIndex = -1;

            for (int i = 0; i < titles.Length; i++)
            {
                if (titles[i] == "Id")
                {
                    idIndex = i;
                    break;
                }
            }

            if (idIndex == -1)
                throw new Exception();

            string[] ids = new string[data.Count() - 1];

            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = data[i + 1][idIndex];
            }

            return ids;
        }

        private static void Insert(Type type)
        {
            var baseEntity = (BaseEntity)Activator.CreateInstance(type);
            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                var relation = (TableRelationAttribute)Attribute.GetCustomAttribute(prop, typeof(TableRelationAttribute));

                if(relation != null)
                {
                    if(SqlConnector.Count(relation.RelationToTable) == 0)
                    {
                        Console.WriteLine("Невозможно добавить данные в эту таблицу, так как одна из родительских таблиц не содержит данных");
                        return;
                    }
                }
            }

            foreach (var prop in properties)
            {
                if (Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute)))
                    continue;

                var relation = (TableRelationAttribute)Attribute.GetCustomAttribute(prop, typeof(TableRelationAttribute));

                if (relation != null)
                {
                    var data = SqlConnector.Select(relation.RelationToTable);

                    while (true)
                    {
                        ShowData(data);
                        Console.WriteLine($"Для заполнения поля '{prop.Name}' введите Id из таблицы '{relation.TableName}'");
                        Console.WriteLine("Введите 0, чтобы отменить добавление.");

                        string rawCommand = Console.ReadLine();

                        if (int.TryParse(rawCommand, out int command) && (AvailableIds(data).Contains(command.ToString()) || command == 0))
                        {
                            if (command == 0)
                                return;

                            prop.SetValue(baseEntity, command);
                            break;
                        }
                        else
                            ShowErrorCommandMessage();
                    }
                }
                else
                {
                    while (true)
                    {

                        Console.WriteLine($"Введите значение для поля '{prop.Name}'");
                        Console.WriteLine("Введите 0, чтобы отменить добавление.");

                        string rawCommand = Console.ReadLine();

                        if (rawCommand == "0")
                            return;

                        if (prop.PropertyType.Name is nameof(Int32))
                        {
                            if (int.TryParse(rawCommand, out int command))
                            {
                                prop.SetValue(baseEntity, command);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Значение должно быть целым числом, повторите ввод.");
                            }
                        }
                        else
                        {
                            prop.SetValue(baseEntity, rawCommand);
                            break;
                        }
                    }
                }
            }

            int result = SqlConnector.Insert(baseEntity);

            if (result > 0)
                Console.WriteLine("Запись добавлена успешно");
            else
                Console.WriteLine("Ошибка добавления");
        }

        private static void Delete(Type type)
        {
            var data = SqlConnector.Select(type);

            if (data.Count > 0)
            {
                while (true)
                {
                    ShowData(data);
                    Console.WriteLine();
                    Console.WriteLine("Введите Id записи для удаления. При наличии связей произойдет каскадное удаление.");
                    Console.WriteLine("Введите 0 для отмены.");

                    string rawCommand = Console.ReadLine();

                    if (int.TryParse(rawCommand, out int command) && (AvailableIds(data).Contains(command.ToString()) || command == 0))
                    {
                        if (command == 0)
                            break;

                        int result = SqlConnector.Delete(type, command);

                        if (result > 0)
                            Console.WriteLine("Успешное удаление.");
                        else
                            Console.WriteLine("Произошла ошибка при удалении");

                        break;
                    }
                    else
                    {
                        ShowErrorCommandMessage();
                        continue;
                    }
                }
            }
            else
                Console.WriteLine("Записей нет. Нечего удалять.");
        }

        #endregion

        static void Main(string[] args)
        {
            Type selectedType = null;

            while (true)
            {
                ShowUserMessage(selectedType);
                string rawCommand = Console.ReadLine();

                if (selectedType is null)
                {
                    if (int.TryParse(rawCommand, out int command) &&
                        (command >= 0 && command <= allTables.Length + 1))
                    {
                        if (command == 0)
                            break;

                        command--;

                        if(command >= 0 && command <= allTables.Length - 1)
                        {
                            selectedType = allTables[command];
                        }
                        else
                        {
                            if(SqlConnector.Count(PharmaTableType) == 0)
                            {
                                Console.WriteLine("Вывод информации невозможен, так как нет созданных аптек");
                            }
                            else
                            {
                                var data = SqlConnector.Select(PharmaTableType);

                                while (true)
                                {
                                    Console.WriteLine("Выбирете Id аптеки для просмотра остатка товаров на складах:");
                                    Console.WriteLine("0 - выход");
                                    ShowData(data);
                                    string rawId = Console.ReadLine();

                                    if (rawId == "0")
                                        break;

                                    if (int.TryParse(rawId, out int id) && AvailableIds(data).Contains(id.ToString()))
                                    {
                                        var viewData = SqlConnector.SelectFromProductRemainingView(id);
                                        Console.WriteLine($"Количество товаров на всех складах аптеки:");
                                        Console.WriteLine();
                                        ShowData(viewData);
                                        Console.WriteLine();
                                        Console.WriteLine("Всего записей: " + (viewData.Count() - 1));

                                        break;
                                    }
                                    else
                                        ShowErrorCommandMessage();
                                }
                            }
                        }
                    }
                    else ShowErrorCommandMessage();
                }
                else
                {
                    if (int.TryParse(rawCommand, out int command) &&
                        (command >= 0 && command <= 3))
                    {
                        switch (command)
                        {
                            case 0:
                                selectedType = null;
                                continue;
                            case 1: Insert(selectedType);
                                break;
                            case 2: Delete(selectedType);
                                break;
                        }
                    }
                    else ShowErrorCommandMessage();
                }
            }
        }


    }
}
