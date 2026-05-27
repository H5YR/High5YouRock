-- Check if DiscourseReactions table exists
SELECT name FROM sqlite_master WHERE type='table' AND name='DiscourseReactions';

-- If you need to delete the migration key to re-run it:
-- DELETE FROM umbracoKeyValue WHERE [key] LIKE '%DiscourseReaction%';

-- View all tables
-- SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;

-- Check migration keys
-- SELECT * FROM umbracoKeyValue WHERE [key] LIKE '%Migration%';
