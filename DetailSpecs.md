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
  * None
* Section C:
  * None
* Section D:
  * Checkbox Hazardous/Non-Haz
* Section E:
  * Generator Name (PeopleID)
  * Generation Date
* Section F:
  * None
* Section G:
  * Received By (PeopleID)
  * Received Date
* On the Fly:
  * Item Volume (L * W * H), unts = ft3, m3)
* Meta Data:
  * Created Date
  * Modified Date
  * UserID (WindowsID)

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
