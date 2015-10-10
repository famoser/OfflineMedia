namespace OfflineMedia.Common.Enums.Generic
{
    /// <summary>
    /// This enum is used inside the EntityMap Attribute, it defines for the converter how an Property of the entity is used in a business Object, and how it is written back to the entity
    /// </summary>
    public enum EntityMappingProcedure
    {
        OnlyRead = 1,
        OnlyWrite = 2,
        ReadAndWrite = 3
    }
}
