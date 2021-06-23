# RWM_Database
Version 1.0 of the RWM Database

## Specifications
* Web Forms / Database App
* SSO Authentication
* Collect/store scanned images of paper forms in the database (desired show preview of PDF full size instead of download)

### Platform
* Visual Studio 2019
* ASP.Net Core, C#
* MySQL
* For Visual Studio, add the following plugin to connect to SLAC MySQL server: https://dev.mysql.com/downloads/windows/visualstudio/
* And, the connector:  https://dev.mysql.com/downloads/connector/net/

### MySQL Connection Properties:
* Database Name: rp_rwm
* Database User: rp_rwm_u (all privileges)
* Password:  See Administrator
* MySQL Server: mysql-dev02
* Port: 3307
* Database Version: MySQL 5.5.54
* Connections allowed from: 134.79.68.169 (Administrator)
* Connections also allowed from:  TBD (Suli Student)

### MySQL Workbench (If Needed):
* Download version 6.3.8 for compatibility with MySQL 5.5.54: https://downloads.mysql.com/archives/workbench/
* 6.3.8:  Windows (x86, 32-bit), MSI Installer

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
* Includes SLAC ID from phonebook
* Have their own properties
* Includes role
* Future expansion to security privlege
* Future expansion to reference people table using SLAC ID.

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
* Amount shipped by quarter, year (cubic feet, weight)
* Amount shipped to each disposal site (cubic feet, weight)
* Incomplete Items
* Progress of ongoing shipment
* Dashboard (# items, # containers (full/empty), etc.)
* Report showing declared items with respect to 18 month limit.

## Workflow

The web-forms application has the following workflow:

* Radioactive waste items are declared in the items view.  In this view, a container may be assigned to each item consistent with the layout of the Declaration Form
* Containers are setup in the containers view.  The view shows a list of items packed in each container.  Containers may be assigned to a shipment.
* Shipments are setup in the shipments view.  The view shows a list of containers which are part of the shipment.  Shipments may be assigned to a burial.
* Burials are setup in the burials view.  The view shows a list of shipments which are part of the burial.  Certificates of disposal may be assigned to a burial.
* A number of views are also provided to setup lists used to pre-populate drop-down boxes, to improve filtering and sorting.
* A number of reports are provided to measure the quantities of waste disposed by time interval
* An attachments view is provided to upload documentation associated which each step.


