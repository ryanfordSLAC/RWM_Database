# Add Detailed Specifications Here
## Items Table
* Declaration Number
* Length (in) INT
* Width (in) INT
* Height (in) INT
* Item_Description VARCHAR (50)
* Generation Location, VARCHAR
* Account Number (#####-#####)
* Section B:
** None
* Section C:
* * None
* Section D:
** Checkbox Hazardous/Non-Haz
* Section E:
** Generator Name (PeopleID)
** Generation Date
* Section F:
** None
* Section G:
** Received By (PeopleID)
** Received Date

## Container Table
* Container Number
* Seal Number
* Type (ContainerTypeID)
* Date Packed
* Packed by (PeopleID)
* Meta Data:  Date Created, UserID (Windows)

## ContainerType
* ID (Primary Key)
* Description
* Inner Volume
* Outer Volume
