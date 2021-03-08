# RWM_Database
Version 1.0 of the RWM Database

## Specifications
* Web Forms / Database App
* SSO Authentication
* Collect/store scanned images of paper forms in the database

## Platform
* Visual Studio
* ASP.Net Core
* MySQL (on Server)

## Main Tables

### Items
* Capture relevant fields from the declaration form
* Have their own properties including Hazard, MaterialType
* Includes a pre-requisite set of details tat allow it to be assigned to a container

### Containers
* Containers are filled with items
* Containers have their own properties, includes Type, Seal Number

### Shipments
* Shipments are filled with containers
* Have their own properties, includes Type, conveyance

### Burials
* Burials are comprised of one or more shipments
* Have their own properties

### People
* Have their own properties
* Includes role
* Future expansion to security privlege

### Attachments
* Upload scanned PDF images or photos
* Includes Type

### List-Type Tables
* List_AttachmentType
* List_ContainerType
* List_ConveyanceType
* List_Hazard
* List_MaterialType
* List_Role
* List_SealNumber
* List_ShipmentType

## Reports
* Amount shipped by quarter (cubic feet, weight)
* Amount shipped to each disposal site (cubic feet, weight)
* Incomplete Items
* Progress of ongoing shipment
* Dashboard (# items, # containers (full/empty), etc.)
