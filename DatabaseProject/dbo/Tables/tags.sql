CREATE TABLE [dbo].[tags] (
    [tag_id]         INT            IDENTITY (1, 1) NOT NULL,
    [name]           NVARCHAR (100) NOT NULL,
    [entity_type_id] INT            NOT NULL,
    [entity_id]      INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([tag_id] ASC),
    CONSTRAINT [FK_Tags_EntityTypes] FOREIGN KEY ([entity_type_id]) REFERENCES [dbo].[entity_types] ([entity_type_id])
);

