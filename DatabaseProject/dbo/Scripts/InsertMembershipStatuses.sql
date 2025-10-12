-- Insert the three core membership statuses into the MembershipStatus table
-- These statuses are used by the GroupMembers smart APIs for workflow management

-- Clear any existing data (optional - remove if you want to keep existing data)
-- DELETE FROM [dbo].[MembershipStatus];

-- Insert the core membership statuses
INSERT INTO [dbo].[MembershipStatus] ([StatusName]) VALUES 
('Pending'),    -- StatusId will be 1 - User requested to join, awaiting approval
('Approved'),   -- StatusId will be 2 - User has been accepted into the group  
('Rejected');   -- StatusId will be 3 - User's join request was denied

-- Verify the data was inserted correctly
SELECT * FROM [dbo].[MembershipStatus] ORDER BY [StatusId];