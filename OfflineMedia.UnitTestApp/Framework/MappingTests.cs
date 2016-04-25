using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.NMModels;
using OfflineMedia.Data.Entities;
using OfflineMedia.Data.Enums;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Framework
{
    [TestClass]
    public class MappingTests
    {
        [TestMethod]
        public void AllPropertiesDefined()
        {
            //arrrange
            var entites = new Dictionary<object, object>()
            {
#pragma warning disable 618
                {new ArticleModel(), new ArticleEntity()},
#pragma warning restore 618
                {new ContentModel(),new ContentEntity()},
                {new GalleryModel(),new GalleryEntity()},
                {new ImageModel(),new ImageEntity()},
                {new RelatedArticleRelationModel(),new RelatedArticleRelations()},
                //new History(),
                {new RelatedThemeRelationModel(),new RelatedThemeRelations()},
                {new SettingModel(),new SettingEntity()},
                {new ThemeArticleRelationModel(),new ThemeArticleRelations()},
                {new ThemeModel(),new ThemeEntity()}
            };
            
            //act
            foreach (var entity in entites)
            {
                //gets all properties of the entity
                Type entityType = entity.Value.GetType();
                IEnumerable<PropertyInfo> entityProps = entityType.GetRuntimeProperties();

                //gets all properties of the business
                Type businessType = entity.Key.GetType();
                IEnumerable<PropertyInfo> businessProps = businessType.GetRuntimeProperties();

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
                            if (attribute.EntityType == null)
                            {
                                //match property of entity to the business
                                var name = attribute.CustomMapping ?? propertyInfo.Name;
                                var prop = entityProps.FirstOrDefault(e => e.Name == name);
                                if (prop != null)
                                {
                                    var conversionAttribute = propertyInfo.GetCustomAttribute(typeof(EntityConversionAttribute), false) as EntityConversionAttribute;
                                    if (conversionAttribute != null)
                                    {
                                        if (prop.PropertyType != conversionAttribute.From || propertyInfo.PropertyType != conversionAttribute.To)
                                            Assert.Fail("Conversion Atrubut malconfigured " +
                                                        ShowInfo(entityType, businessType, prop, propertyInfo));

                                    }
                                    else
                                    {
                                        if (prop.PropertyType.IsConstructedGenericType)
                                        {
                                            if (!propertyInfo.PropertyType.IsConstructedGenericType)
                                                Assert.Fail("Not both properties are Genric Types " +
                                                            ShowInfo(entityType, businessType, prop, propertyInfo));
                                            else
                                            {
                                                if (prop.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                                                {
                                                    if (!propertyInfo.PropertyType.IsConstructedGenericType)
                                                        Assert.Fail("Not both properties are Nullable Types " +
                                                                    ShowInfo(entityType, businessType, prop,
                                                                        propertyInfo));
                                                    else
                                                    {
                                                        if (prop.PropertyType.GenericTypeArguments[0] !=
                                                            propertyInfo.PropertyType.GenericTypeArguments[0])
                                                            Assert.Fail(
                                                                "Not both properties have the same Nullable Base Type " +
                                                                ShowInfo(entityType, businessType, prop, propertyInfo));
                                                    }
                                                }
                                            }
                                        }
                                        else if (prop.PropertyType != propertyInfo.PropertyType)
                                            Assert.Fail("Not both properties have the same Type " +
                                                        ShowInfo(entityType, businessType, prop, propertyInfo));
                                    }
                                }
                                else if (!attribute.IsOptional)
                                {
                                    Assert.Fail("Attribut missung in Entity which is requested in business " + ShowInfo(entityType, businessType, null, propertyInfo));
                                }
                            }
                            else
                            {
                                Assert.Fail("Test not valid for this configuration");
                            }
                        }
                    }
                }
            }
        }

        public string ShowInfo(Type entityType, Type businessType, PropertyInfo entityprop, PropertyInfo businessprop)
        {
            var res = "";
            if (entityType != null)
                res += "EntityType: " + entityType.Name + "; ";
            if (businessType != null)
                res += "BusinessType: " + businessType.Name + "; ";
            if (entityprop != null)
                res += "EntityProp: " + entityprop.Name + "; ";
            if (businessprop != null)
                res += "BusinessProp: " + businessprop.Name + "; ";
            return res;
        }
    }
}

