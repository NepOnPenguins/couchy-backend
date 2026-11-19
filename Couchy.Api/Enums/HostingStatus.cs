namespace Couchy.Api.Enums;

// Names match the Java enum: they are stored in the database and used in JSON as-is.
public enum HostingStatus
{
    MAY_HOST,
    CAN_HOST,
    CANT_HOST
}
