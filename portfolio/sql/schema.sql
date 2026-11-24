-- schema.sql : the portfolio database
DROP SCHEMA IF EXISTS portfolio CASCADE;
CREATE SCHEMA portfolio;

CREATE TABLE portfolio.course (
  course_id    BIGINT        GENERATED ALWAYS AS IDENTITY,
  code         VARCHAR(20)   NOT NULL,
  title        VARCHAR(120)  NOT NULL,
  term         VARCHAR(40),
  description  TEXT,
  CONSTRAINT pk_course       PRIMARY KEY (course_id),
  CONSTRAINT uq_course_code  UNIQUE (code)
);

CREATE TABLE portfolio.project (
  project_id    BIGINT        GENERATED ALWAYS AS IDENTITY,
  course_id     BIGINT        NOT NULL,
  title         VARCHAR(120)  NOT NULL,
  summary       VARCHAR(300),
  description   TEXT,
  repo_url      VARCHAR(300),
  video_url     VARCHAR(300),
  started_on    DATE          NOT NULL DEFAULT CURRENT_DATE,
  completed_on  DATE,
  CONSTRAINT pk_project         PRIMARY KEY (project_id),
  CONSTRAINT fk_project_course  FOREIGN KEY (course_id)
    REFERENCES portfolio.course (course_id) ON DELETE RESTRICT,
  CONSTRAINT chk_project_dates  CHECK (completed_on IS NULL OR completed_on >= started_on)
);

CREATE TABLE portfolio.project_image (
  image_id    BIGINT        GENERATED ALWAYS AS IDENTITY,
  project_id  BIGINT        NOT NULL,
  url         VARCHAR(500)  NOT NULL,
  caption     VARCHAR(200),
  sort_order  INTEGER       NOT NULL DEFAULT 0,
  CONSTRAINT pk_project_image        PRIMARY KEY (image_id),
  CONSTRAINT fk_image_project        FOREIGN KEY (project_id)
    REFERENCES portfolio.project (project_id) ON DELETE CASCADE,
  CONSTRAINT uq_image_project_url    UNIQUE (project_id, url),
  CONSTRAINT chk_image_url           CHECK (url LIKE 'http%')
);s