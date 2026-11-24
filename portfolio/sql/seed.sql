INSERT INTO portfolio.course (code, title, term, description) VALUES
  ('DBMWEB1', 'Database Management and Web Systems', '1st Sem 2026-2027', 'SQL, ASP.NET Core MVC and PostgreSQL.'),
  ('WEBDES1', 'Web Design', '1st Sem 2026-2027', 'HTML, CSS and layout.');

INSERT INTO portfolio.project (course_id, title, summary, repo_url, video_url, started_on, completed_on) VALUES
  (1, 'Library System', 'Books, members, loans and notes.', 'https://github.com/beplt/library', NULL, '2026-10-01', '2026-11-10'),
  (1, 'Portfolio', 'This site.', NULL, NULL, '2026-11-20', NULL),
  (2, 'LONA Website', 'A website for the LONA project.', NULL, NULL, '2026-08-22', '2026-09-30');

INSERT INTO portfolio.project_image (project_id, url, caption, sort_order) VALUES
  (1, 'https://picsum.photos/seed/library1/800/450', 'Books page', 1),
  (1, 'https://picsum.photos/seed/library2/800/450', 'Members page', 2),
  (3, 'https://picsum.photos/seed/lona1/800/450', 'Home page', 1);

SELECT (SELECT COUNT(*) FROM portfolio.course) AS courses,
       (SELECT COUNT(*) FROM portfolio.project) AS projects,
       (SELECT COUNT(*) FROM portfolio.project_image) AS images;