-- Script to add unique constraint to prevent duplicate group memberships
-- This ensures that a user can only have one membership record per group

USE DatabaseProject;

-- First, check if the constraint already exists
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes i
    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE i.object_id = OBJECT_ID('GroupMembers')
    AND i.is_unique_constraint = 1
    AND c.name IN ('GroupId', 'UserId')
)
BEGIN
    -- Add unique constraint on (GroupId, UserId)
    ALTER TABLE GroupMembers
    ADD CONSTRAINT UQ_GroupMembers_GroupUser UNIQUE (GroupId, UserId);

    PRINT 'Unique constraint UQ_GroupMembers_GroupUser added successfully.';
END
ELSE
BEGIN
    PRINT 'Unique constraint already exists.';
END

-- Verify the constraint
SELECT
    i.name as ConstraintName,
    i.type_desc as ConstraintType,
    c.name as ColumnName
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('GroupMembers')
AND i.is_unique_constraint = 1
ORDER BY i.name, ic.key_ordinal;