use DatabaseProject

DROP TABLE IF EXISTS tags;
DROP TABLE IF EXISTS availability_slots;
DROP TABLE IF EXISTS group_members;
DROP TABLE IF EXISTS study_groups;
DROP TABLE IF EXISTS courses;
DROP TABLE IF EXISTS users;


-- -------------------
-- Users Table
-- -------------------
CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255) NOT NULL,
    email NVARCHAR(255) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    skills NVARCHAR(MAX),  -- storing JSON or comma-separated tags
    visibility BIT DEFAULT 1,
    bio NVARCHAR(MAX)
);

-- -------------------
-- Courses Table
-- -------------------
CREATE TABLE courses (
    course_id INT IDENTITY(1,1) PRIMARY KEY,
    course_code NVARCHAR(50) NOT NULL,
    course_name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX)
);

-- -------------------
-- Study Groups Table
-- -------------------
CREATE TABLE study_groups (
    group_id INT IDENTITY(1,1) PRIMARY KEY,
    course_id INT NOT NULL,
    creator_id INT NOT NULL,
    topic NVARCHAR(255),
    time_slot NVARCHAR(MAX),
    description NVARCHAR(MAX),
    CONSTRAINT FK_StudyGroups_Courses FOREIGN KEY (course_id)
        REFERENCES courses(course_id) ON DELETE CASCADE,
    CONSTRAINT FK_StudyGroups_Users FOREIGN KEY (creator_id)
        REFERENCES users(user_id) ON DELETE NO ACTION
);

-- -------------------
-- Group Members Table
-- -------------------
CREATE TABLE group_members (
    group_member_id INT IDENTITY(1,1) PRIMARY KEY,
    group_id INT NOT NULL,
    user_id INT NOT NULL,
    status NVARCHAR(10) NOT NULL CHECK (status IN ('pending','approved','rejected')),
    CONSTRAINT FK_GroupMembers_Groups FOREIGN KEY (group_id)
        REFERENCES study_groups(group_id) ON DELETE CASCADE,
    CONSTRAINT FK_GroupMembers_Users FOREIGN KEY (user_id)
        REFERENCES users(user_id) ON DELETE CASCADE
);

-- -------------------
-- Availability Slots Table
-- -------------------
CREATE TABLE availability_slots (
    availability_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    day_of_week NVARCHAR(3) NOT NULL CHECK (day_of_week IN ('Mon','Tue','Wed','Thu','Fri','Sat','Sun')),
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    CONSTRAINT FK_AvailabilitySlots_Users FOREIGN KEY (user_id)
        REFERENCES users(user_id) ON DELETE CASCADE
);

-- -------------------
-- Tags Table
-- -------------------
CREATE TABLE tags (
    tag_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    entity_type NVARCHAR(10) NOT NULL CHECK (entity_type IN ('user','group')),
    entity_id INT NOT NULL
    -- Polymorphic: entity_id refers to either users.user_id or study_groups.group_id
);
