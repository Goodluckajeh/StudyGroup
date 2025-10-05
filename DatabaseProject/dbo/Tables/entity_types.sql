CREATE TABLE [dbo].[entity_types] (
    [entity_type_id]   INT           IDENTITY (1, 1) NOT NULL,
    [entity_type_name] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([entity_type_id] ASC),
    UNIQUE NONCLUSTERED ([entity_type_name] ASC)
);

