using System;
using System.Reflection;
using OfflineMediaV3.Common.Enums.Generic;
using OfflineMediaV3.Common.Framework.Converters;

namespace OfflineMediaV3.Common.Framework
{


    /// <summary>
    /// This attribute defines, which properties of the entity is supposed to be the primary key used by the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityPrimaryKeyAttribute : System.Attribute
    {
    }

    /// <summary>
    /// This attribute defines, which methods of a business object is supposed to be called before saving / conversion into entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallBeforeSaveAttribute : System.Attribute
    {
    }

    /// <summary>
    /// This attribute defines, which properties of an entity are supposed to be written into the business object and vice versa
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityTableMapAttribute : System.Attribute
    {
        public Type[] Types { get; private set; }

        public EntityTableMapAttribute(params Type[] types)
        {
            Types = types;
        }
    }

    /// <summary>
    /// This attribute defines, which properties of an entity are supposed to be written into the business object and vice versa
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityMapAttribute : System.Attribute
    {
        public EntityMappingProcedure Procedure { get; private set; }
        public string CustomMapping { get; private set; }
        public string EntityType { get; private set; }
        public bool IsOptional { get; private set; }

        public EntityMapAttribute(EntityMappingProcedure procedure = EntityMappingProcedure.ReadAndWrite, string customMapping = null, string entityType = null, bool isOptional = false)
        {
            Procedure = procedure;
            CustomMapping = customMapping;
            IsOptional = isOptional;
            EntityType = entityType;
        }
    }

    /// <summary>
    /// This attribute defines, how a property of an entity is supposed to be converted before writing it the business object and vice versa
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityConversionAttribute : System.Attribute
    {
        public Type From { get; set; }
        public Type To { get; set; }
        public IEntityValueConverter Converter { get; private set; }

        public EntityConversionAttribute(Type from, Type to)
        {
            From = from;
            To = to;

            if (from == typeof(string) && to == typeof(Guid))
                Converter = new StringGuidConverter();
            else if (from == typeof(string) && to == typeof(Uri))
                Converter = new StringUriConverter();
            else if (from == typeof(string) && to == typeof(bool))
                Converter = new StringBoolConverter();
            else if (from == typeof(int) && to.GetTypeInfo().IsEnum)
            {
                Type repo = typeof(EnumConverter<>);
                Type[] args = { to };
                Type constructed = repo.MakeGenericType(args);

                Converter = Activator.CreateInstance(constructed) as IEntityValueConverter;
            }
            else
                //Unknown Converter: This Converter will throw an Exception as soon as conversion is attempted the first time
                Converter = new DummyConverter();
        }
    }
}
