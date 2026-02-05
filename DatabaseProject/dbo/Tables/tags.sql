CREATE TABLE [dbo].[Tags] (
    [TagId]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NOT NULL,
    [EntityTypeId] INT            NOT NULL,
    [EntityId]     INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([TagId] ASC),
    CONSTRAINT [FK_Tags_EntityTypes] FOREIGN KEY ([EntityTypeId]) REFERENCES [dbo].[EntityTypes] ([EntityTypeId])
);

