USE DatabaseProject;

-- Drop tables if they exist
DROP TABLE IF EXISTS tags;
DROP TABLE IF EXISTS availability_slots;
DROP TABLE IF EXISTS group_members;
DROP TABLE IF EXISTS study_groups;
DROP TABLE IF EXISTS courses;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS membership_status;
DROP TABLE IF EXISTS days_of_week;
DROP TABLE IF EXISTS entity_types;

-- -------------------
-- Lookup Tables
-- -------------------

-- Membership Status
CREATE TABLE membership_status (
    status_id INT IDENTITY(1,1) PRIMARY KEY,
    status_name NVARCHAR(20) NOT NULL UNIQUE
);

INSERT INTO membership_status (status_name)
VALUES ('pending'), ('approved'), ('rejected');

-- Days of Week
CREATE TABLE days_of_week (
    day_id INT IDENTITY(1,1) PRIMARY KEY,
    day_name NVARCHAR(10) NOT NULL UNIQUE
);

INSERT INTO days_of_week (day_name)
VALUES ('Mon'), ('Tue'), ('Wed'), ('Thu'), ('Fri'), ('Sat'), ('Sun');

-- Entity Types
CREATE TABLE entity_types (
    entity_type_id INT IDENTITY(1,1) PRIMARY KEY,
    entity_type_name NVARCHAR(20) NOT NULL UNIQUE
);

INSERT INTO entity_types (entity_type_name)
VALUES ('user'), ('group');

-- -------------------
-- Users Table
-- -------------------
CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    first_name NVARCHAR(255) NOT NULL,
    last_name NVARCHAR(255) NOT NULL,
    email NVARCHAR(255) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    skills NVARCHAR(MAX),
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
    status_id INT NOT NULL,
    CONSTRAINT FK_GroupMembers_Groups FOREIGN KEY (group_id)
        REFERENCES study_groups(group_id) ON DELETE CASCADE,
    CONSTRAINT FK_GroupMembers_Users FOREIGN KEY (user_id)
        REFERENCES users(user_id) ON DELETE CASCADE,
    CONSTRAINT FK_GroupMembers_Status FOREIGN KEY (status_id)
        REFERENCES membership_status(status_id) ON DELETE no action
);

-- -------------------
-- Availability Slots Table
-- -------------------
CREATE TABLE availability_slots (
    availability_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    day_id INT NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    CONSTRAINT FK_AvailabilitySlots_Users FOREIGN KEY (user_id)
        REFERENCES users(user_id) ON DELETE CASCADE,
    CONSTRAINT FK_AvailabilitySlots_Days FOREIGN KEY (day_id)
        REFERENCES days_of_week(day_id) ON DELETE no action
);

-- -------------------
-- Tags Table
-- -------------------
CREATE TABLE tags (
    tag_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    entity_type_id INT NOT NULL,
    entity_id INT NOT NULL,
    CONSTRAINT FK_Tags_EntityTypes FOREIGN KEY (entity_type_id)
        REFERENCES entity_types(entity_type_id) ON DELETE no action
);
