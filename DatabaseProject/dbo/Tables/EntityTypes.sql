CREATE TABLE [dbo].[EntityTypes] (
    [EntityTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [EntityTypeName] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([EntityTypeId] ASC),
    UNIQUE NONCLUSTERED ([EntityTypeName] ASC)
);

