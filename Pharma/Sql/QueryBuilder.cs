using Pharma.Attributes;
using Pharma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharma.Sql
{
    public static class QueryBuilder
    {
        public static string GenerateAddScript(BaseEntity entity)
        {
            var propValues = new Dictionary<string, object>();
            var properties = entity.GetType().GetProperties();

            foreach(var prop in properties)
            {
                if (Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute)))
                    continue;

                propValues.Add(prop.Name, prop.GetValue(entity));
            }

            string values = "";

            foreach (var tKeyTValue in propValues)
            {
                if (tKeyTValue.Value is string)
                    values += $"N'{tKeyTValue.Value}', ";
                else values += $"{tKeyTValue.Value}, ";
            }

            return $"INSERT INTO [{entity.GetType().Name}] ({string.Join(",", propValues.Keys.ToArray())}) " +
                $"VALUES ({values.Substring(0,values.Length - 2)})";
        }

        public static string GenerateSelectScript(Type type)
        {
            StringBuilder select = new StringBuilder();
            string innerJoins = string.Empty;
            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                if (Attribute.IsDefined(prop, typeof(TableRelationAttribute)))
                {
                    var relation = (TableRelationAttribute)Attribute.GetCustomAttribute(prop, typeof(TableRelationAttribute));

                    select.Append($"[{relation.TableName}].[{relation.Description}] AS {prop.Name},");
                    innerJoins += $"INNER JOIN [{relation.TableName}] ON [{prop.Name}] = [{relation.TableName}].[Id] ";
                }
                else
                {
                    select.Append($"[{type.Name}].[{prop.Name}],");
                }
            }

            select.Remove(select.Length - 1, 1);
            string query = $"SELECT {select} FROM {type.Name} {innerJoins} ";
            return query;
        }
        public static string GenerateCountScript(Type type) => 
            $"SELECT COUNT(*) FROM [{type.Name}]";

        public static string GenerateDeleteScript(Type type, int idToRemove) =>
             $"DELETE FROM [{type.Name}] WHERE Id = {idToRemove}";


        public static string GenerateSelectFromProductRemainingView(int idPharmacy) =>
            $"SELECT * FROM ProductRemainingView WHERE PharmacyId = {idPharmacy}";

    }
}
