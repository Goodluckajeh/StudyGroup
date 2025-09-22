CREATE TABLE [dbo].[tags] (
    [tag_id]      INT            IDENTITY (1, 1) NOT NULL,
    [name]        NVARCHAR (100) NOT NULL,
    [entity_type] NVARCHAR (10)  NOT NULL,
    [entity_id]   INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([tag_id] ASC),
    CHECK ([entity_type]='group' OR [entity_type]='user')
);

