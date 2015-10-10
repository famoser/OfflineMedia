using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OfflineMedia.Common.Enums.Generic;
using OfflineMedia.Common.Framework;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Singleton;

namespace OfflineMedia.Business.Framework.Generic
{
    public class EntityModelConverter : SingletonBase<EntityModelConverter>
    {
        /// <summary>
        /// Writes all properties which are marked with the EntityMapAttribute in the Business Object from the Entity to the Business object.
        /// </summary>
        /// <param name="business">the Entity object, the "source"</param>
        /// <param name="entity">the Business object, the target</param>
        /// <returns>the Business object (same instance as passed)</returns>
        public TB ConvertToBusiness<TB, TE>(TE entity, TB business)
            where TB : class
            where TE : class
        {
            if (entity == null || business == null)
                return business;

            //gets all properties of the entity
            Type entityType = entity.GetType();
            IEnumerable<PropertyInfo> entityProps = entityType.GetRuntimeProperties();

            //gets all properties of the business
            IEnumerable<PropertyInfo> businessProps = typeof(TB).GetRuntimeProperties();

            foreach (var propertyInfo in businessProps)
            {
                //check for the attribute
                var attribute = propertyInfo.GetCustomAttribute(typeof(EntityMapAttribute), false) as EntityMapAttribute;
                if (attribute != null)
                {
                    //check if I am allowed to write properties of entity to this object
                    if (attribute.Procedure == EntityMappingProcedure.ReadAndWrite ||
                        attribute.Procedure == EntityMappingProcedure.OnlyRead)
                    {
                        //match entity 
                        if (attribute.EntityType == null || CheckIfTypeAndNameMatch(entityType,attribute.EntityType))
                        {
                            //match property of entity to the business
                            var name = attribute.CustomMapping ?? propertyInfo.Name;
                            var prop = entityProps.FirstOrDefault(e => e.Name == name);
                            if (prop != null)
                            {
                                propertyInfo.SetValue(business, GetValue(propertyInfo, false, prop.GetValue(entity)));
                            }
                            else if (!attribute.IsOptional)
                            {
                                //if Property not found on entity, but requested on business, we throw an error
                                string errorMsg = String.Format("ConvertToBusiness(): Property '{0}' which should be mapped to '{1}' was not found on entity, but requested in the business object.", propertyInfo.Name, name);
                                errorMsg += String.Format("typeof business: '{0}',  typeof entity: '{1}'", business.GetType(), entity.GetType());
                                LogHelper.Instance.Log(LogLevel.Error,this, errorMsg);
                            }
                        }
                    }
                }
            }

            return business;
        }

        public List<TB> ConvertAllToBusiness<TB, TE>(List<TE> entity)
            where TB : class, new() where TE : class
        {
            var res = new List<TB>();
            for (int i = 0; i < entity.Count; i++)
            {
                res.Add(ConvertToBusiness(entity[0], new TB()));
            }
            return res;
        }

        private bool CheckIfTypeAndNameMatch(Type type, string name)
        {
            if (type.Name == name)
                return true;
            //while (type.DeclaringType != typeof(object) && type.BaseType != null)
            //{
            //    type = type.BaseType;
            //    if (type.Name == name)
            //        return true;
            //}
            return false;
        }

        /// <summary>
        /// Writes all properties which are marked with the EntityMapAttribute from the Business object to the Entity object.
        /// </summary>
        /// <param name="business">the Business object, the "source"</param>
        /// <param name="entity">the Entity object, the target</param>
        /// <param name="takeCareOfDateTime">if this is set to true, the ChangeDate of the entity (If it exists) is set to the current time</param>
        /// <param name="entityIsToBeCreated">if this is set and true, the CreateDate of the entity is set to the current time</param>
        /// <returns>the Entity object (same instance as passed)</returns>
        public TE ConvertToEntity<TE, TB>(TB business, TE entity, bool takeCareOfDateTime = false)
            where TE : class
            where TB : class
        {
            if (entity == null || business == null)
                return entity;

            //Check if class contains a method to be called before conversion
            Type businessType = business.GetType();
            IEnumerable<MethodInfo> businessMethods = businessType.GetRuntimeMethods();

            foreach (var businessMethod in businessMethods)
            {
                var attribute = businessMethod.GetCustomAttribute(typeof(CallBeforeSaveAttribute), false) as CallBeforeSaveAttribute;
                if (attribute != null)
                    businessMethod.Invoke(business, null);
            }

            //gets all properties of the entity
            Type entityType = typeof(TE);
            IEnumerable<PropertyInfo> entityProps = entityType.GetRuntimeProperties();

            //gets all properties of the business
            IEnumerable<PropertyInfo> businessProps = businessType.GetRuntimeProperties();

            foreach (var propertyInfo in businessProps)
            {
                //check for the attribute
                var attribute = propertyInfo.GetCustomAttribute(typeof(EntityMapAttribute), false) as EntityMapAttribute;
                if (attribute != null)
                {
                    //check if I am allowed to write properties to the entity from this object
                    if (attribute.Procedure == EntityMappingProcedure.ReadAndWrite ||
                        attribute.Procedure == EntityMappingProcedure.OnlyWrite)
                    {
                        //match entity 
                        if (attribute.EntityType == null || CheckIfTypeAndNameMatch(entityType, attribute.EntityType))
                        {
                            //match property of entity to the business
                            var name = attribute.CustomMapping ?? propertyInfo.Name;
                            var prop = entityProps.FirstOrDefault(e => e.Name == name);
                            if (prop != null)
                            {
                                prop.SetValue(entity, GetValue(propertyInfo, true, propertyInfo.GetValue(business)));

                                if (takeCareOfDateTime)
                                {
                                    if (prop.Name == "ChangeDate")
                                        prop.SetValue(entity, DateTime.Now);
                                    if (prop.Name == "CreateDate")
                                    {
                                        if ((DateTime)GetValue(propertyInfo, true, propertyInfo.GetValue(business)) == DateTime.MinValue)
                                        prop.SetValue(entity, DateTime.Now);
                                    }
                                }
                            }
                            else
                            {
                                if (!attribute.IsOptional)
                                {
                                    //if Property not found on entity, but requested on business, we throw an error
                                    string errorMsg = String.Format("ConvertToEntity(): Property '{0}' which should be mapped to '{1}' was not found on entity, but requested in the business object.", propertyInfo.Name, name);
                                    errorMsg += String.Format("typeof business: '{0}',  typeof entity: '{1}'", business.GetType(), entity.GetType());
                                    LogHelper.Instance.Log(LogLevel.Error, this, errorMsg);
                                }
                            }
                        }
                    }
                }
            }

            return entity;
        }

        public bool FillProperties<TE, TB>(ref TB business, TE entity)
            where TE : class
            where TB : class
        {
            if (entity == null || business == null)
                return false;

            //gets all properties of the entity
            Type entityType = entity.GetType();
            IEnumerable<PropertyInfo> entityProps = entityType.GetRuntimeProperties();

            //gets all properties of the business
            IEnumerable<PropertyInfo> businessProps = typeof(TB).GetRuntimeProperties();

            foreach (var propertyInfo in businessProps)
            {
                //check for the attribute
                var attribute = propertyInfo.GetCustomAttribute(typeof(EntityMapAttribute), false) as EntityMapAttribute;
                if (attribute != null)
                {
                    //check if I am allowed to write properties of entity to this object
                    if (attribute.Procedure == EntityMappingProcedure.ReadAndWrite ||
                        attribute.Procedure == EntityMappingProcedure.OnlyRead)
                    {
                        //match entity 
                        if (attribute.EntityType == null || CheckIfTypeAndNameMatch(entityType, attribute.EntityType))
                        {
                            //match property of entity to the business
                            var name = attribute.CustomMapping ?? propertyInfo.Name;
                            var prop = entityProps.FirstOrDefault(e => e.Name == name);
                            if (prop != null)
                            {
                                propertyInfo.SetValue(business, GetValue(propertyInfo, false, prop.GetValue(entity)));
                            }
                            else if (!attribute.IsOptional)
                            {
                                //if Property not found on entity, but requested on business, we throw an error
                                string errorMsg = String.Format("FillProperties(): Property '{0}' which should be mapped to '{1}' was not found on entity, but requested in the business object.", propertyInfo.Name, name);
                                errorMsg += String.Format("typeof business: '{0}',  typeof entity: '{1}'", business.GetType(), entity.GetType());
                                LogHelper.Instance.Log(LogLevel.Error, this, errorMsg);
                            }
                        }
                    }
                }
            }

            return true;
        }

        public int GetPrimaryKeyFromBusiness<TB>(TB business) where TB : class
        {
            //gets all properties of the business
            Type businessType = business.GetType();
            IEnumerable<PropertyInfo> businessProps = businessType.GetRuntimeProperties();

            foreach (var propertyInfo in businessProps)
            {
                //check for the attribute
                var attribute = propertyInfo.GetCustomAttribute(typeof(EntityPrimaryKeyAttribute), false) as EntityPrimaryKeyAttribute;
                if (attribute != null)
                {
                    return (int)propertyInfo.GetValue(business);
                }
            }

            //if Property not found on entity, but requested on business, we throw an error
            string errorMsg = String.Format("GetPrimaryKey(): EntityPrimaryKeyAttribute was not found on business. typeof business: '{0}'", business.GetType());
            LogHelper.Instance.Log(LogLevel.Error, this, errorMsg);

            return -1;
        }

        public bool SetPrimaryKeyToBusiness<TB>(TB business, object primaryKey) where TB : class
        {
            //gets all properties of the business
            Type businessType = business.GetType();
            IEnumerable<PropertyInfo> businessProps = businessType.GetRuntimeProperties();

            foreach (var propertyInfo in businessProps)
            {
                //check for the attribute
                var attribute = propertyInfo.GetCustomAttribute(typeof(EntityPrimaryKeyAttribute), false) as EntityPrimaryKeyAttribute;
                if (attribute != null)
                {
                    propertyInfo.SetValue(business,primaryKey);
                }
            }

            //if Property not found on entity, but requested on business, we throw an error
            string errorMsg = String.Format("GetPrimaryKey(): EntityPrimaryKeyAttribute was not found on business. typeof business: '{0}'", business.GetType());
            LogHelper.Instance.Log(LogLevel.Error, this, errorMsg);

            return true;
        }

        public string GetPrimaryKeyNameFromBusiness<TB>() where TB : class
        {
            //gets all properties of the business
            IEnumerable<PropertyInfo> businessProps = typeof(TB).GetRuntimeProperties();

            foreach (var propertyInfo in businessProps)
            {
                //check for the attribute
                var attribute = propertyInfo.GetCustomAttribute(typeof(EntityPrimaryKeyAttribute), false) as EntityPrimaryKeyAttribute;
                if (attribute != null)
                {
                    return propertyInfo.Name;
                }
            }

            return null;
        }

        public string GetKeyNameFromBusiness<TB>(string propertyName) where TB : class
        {
            //gets all properties of the business
            IEnumerable<PropertyInfo> businessProps = typeof(TB).GetRuntimeProperties();

            foreach (var propertyInfo in businessProps)
            {
                if (propertyInfo.Name == propertyName)
                {
                    return GetEntityName(propertyInfo);
                }
            }
            return propertyName;
        }

        public string GetEntityName(PropertyInfo propertyInfo)
        {
            //check for the attribute
            var attribute = propertyInfo.GetCustomAttribute(typeof(EntityMapAttribute), false) as EntityMapAttribute;
            if (attribute != null && attribute.CustomMapping != null)
            {
                return attribute.CustomMapping;
            }
            return propertyInfo.Name;
        }

        public object GetValue(PropertyInfo propertyInfo, bool fromBuiness, object value)
        {
            //Convert value of requested
            var conversionAttribute =
                propertyInfo.GetCustomAttribute(typeof(EntityConversionAttribute), false) as EntityConversionAttribute;
            if (conversionAttribute != null)
            {
                if (fromBuiness)
                    value = conversionAttribute.Converter.ConvertBack(value);
                else
                    value = conversionAttribute.Converter.Convert(value);
            }

            return value;
        }
    }
}
