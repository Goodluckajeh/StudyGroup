-- Script to remove duplicate group memberships
-- This script identifies and removes duplicate entries where the same user is a member of the same group multiple times
-- It keeps the membership with the lowest GroupMemberId (earliest join) and removes the others

USE DatabaseProject;

-- First, let's see what duplicates exist
SELECT
    gm.GroupId,
    gm.UserId,
    COUNT(*) as duplicate_count,
    STRING_AGG(CAST(gm.GroupMemberId AS VARCHAR), ', ') as member_ids,
    STRING_AGG(ms.StatusName, ', ') as statuses
FROM GroupMembers gm
INNER JOIN MembershipStatus ms ON gm.StatusId = ms.StatusId
GROUP BY gm.GroupId, gm.UserId
HAVING COUNT(*) > 1
ORDER BY gm.GroupId, gm.UserId;

-- Create a temporary table to store the IDs to delete
-- We'll keep the record with the smallest GroupMemberId for each (GroupId, UserId) pair
WITH DuplicateGroups AS (
    SELECT
        GroupId,
        UserId,
        MIN(GroupMemberId) as keep_id
    FROM GroupMembers
    GROUP BY GroupId, UserId
    HAVING COUNT(*) > 1
)
SELECT gm.GroupMemberId
INTO #MembersToDelete
FROM GroupMembers gm
INNER JOIN DuplicateGroups dg ON gm.GroupId = dg.GroupId AND gm.UserId = dg.UserId
WHERE gm.GroupMemberId != dg.keep_id;

-- Show what will be deleted
SELECT
    gm.*,
    ms.StatusName,
    'TO BE DELETED' as action
FROM GroupMembers gm
INNER JOIN MembershipStatus ms ON gm.StatusId = ms.StatusId
INNER JOIN #MembersToDelete d ON gm.GroupMemberId = d.GroupMemberId
ORDER BY gm.GroupId, gm.UserId, gm.GroupMemberId;

-- Delete the duplicate memberships
DELETE gm
FROM GroupMembers gm
INNER JOIN #MembersToDelete d ON gm.GroupMemberId = d.GroupMemberId;

-- Clean up temporary table
DROP TABLE #MembersToDelete;

-- Verify the cleanup
SELECT
    gm.GroupId,
    gm.UserId,
    COUNT(*) as remaining_count,
    STRING_AGG(CAST(gm.GroupMemberId AS VARCHAR), ', ') as remaining_member_ids,
    STRING_AGG(ms.StatusName, ', ') as statuses
FROM GroupMembers gm
INNER JOIN MembershipStatus ms ON gm.StatusId = ms.StatusId
GROUP BY gm.GroupId, gm.UserId
HAVING COUNT(*) > 1
ORDER BY gm.GroupId, gm.UserId;

-- If there are still duplicates, it means they have different statuses
-- In that case, we should prioritize 'Approved' over 'Pending' over 'Rejected'
WITH StatusPriority AS (
    SELECT
        gm.GroupId,
        gm.UserId,
        gm.GroupMemberId,
        ms.StatusName,
        CASE
            WHEN ms.StatusName = 'Approved' THEN 1
            WHEN ms.StatusName = 'Pending' THEN 2
            WHEN ms.StatusName = 'Rejected' THEN 3
        END as priority
    FROM GroupMembers gm
    INNER JOIN MembershipStatus ms ON gm.StatusId = ms.StatusId
),
DuplicateWithPriority AS (
    SELECT
        GroupId,
        UserId,
        MIN(priority) as best_priority
    FROM StatusPriority
    GROUP BY GroupId, UserId
    HAVING COUNT(*) > 1
)
SELECT sp.GroupMemberId
INTO #MembersToDeleteFinal
FROM StatusPriority sp
INNER JOIN DuplicateWithPriority dp ON sp.GroupId = dp.GroupId AND sp.UserId = dp.UserId
WHERE sp.priority != dp.best_priority;

-- Delete based on status priority
DELETE gm
FROM GroupMembers gm
INNER JOIN #MembersToDeleteFinal d ON gm.GroupMemberId = d.GroupMemberId;

-- Final verification
SELECT
    gm.GroupId,
    gm.UserId,
    COUNT(*) as final_count,
    STRING_AGG(CAST(gm.GroupMemberId AS VARCHAR), ', ') as member_ids,
    STRING_AGG(ms.StatusName, ', ') as statuses
FROM GroupMembers gm
INNER JOIN MembershipStatus ms ON gm.StatusId = ms.StatusId
GROUP BY gm.GroupId, gm.UserId
HAVING COUNT(*) > 1
ORDER BY gm.GroupId, gm.UserId;

-- Clean up
DROP TABLE IF EXISTS #MembersToDeleteFinal;

PRINT 'Duplicate group membership cleanup completed.';