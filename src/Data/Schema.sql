CREATE TABLE World (
    WorldId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Name varchar(255) NOT NULL UNIQUE,
	CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
);

CREATE TABLE Location (
    LocationId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    WorldId int NOT NULL FOREIGN KEY REFERENCES World(WorldId),
    Name varchar(255) NOT NULL,
	Nationality varchar(255) NOT NULL,
	Population int NOT NULL,
	Climate varchar(255) NOT NULL,
	Terrain varchar(255) NOT NULL,
	CreatedAt DATETIME NOT NULL DEFAULT(GETDATE()),
	CONSTRAINT UC_Location_WorldId_Name UNIQUE (WorldId, Name)
);

CREATE TABLE AreaType (
    AreaTypeId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Name varchar(255) NOT NULL UNIQUE,
	CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
);

CREATE TABLE Area (
    AreaId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    LocationId int NOT NULL FOREIGN KEY REFERENCES Location(LocationId),
	AreaTypeId int NOT NULL FOREIGN KEY REFERENCES AreaType(AreaTypeId),
    Name varchar(255) NOT NULL,
	Size int NOT NULL,
	CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
);
