-- Seed each existing park (that has no photos yet) with 8 sport-themed photos.
-- Uses LoremFlickr with per-photo tags (football pitch, tennis court, basketball
-- court, stadium, track, etc.) and a deterministic ?lock seed so the same URL
-- resolves to the same Flickr image on every load.
--
-- Safe to re-run: only parks with zero photos receive seeds. To wipe and reseed:
--   DELETE FROM ParkPhotos;  -- then run this script again.

DECLARE @now DATETIMEOFFSET = SYSDATETIMEOFFSET();

WITH parks_to_seed AS (
    SELECT p.Id
    FROM Parks p
    LEFT JOIN ParkPhotos ph ON ph.ParkId = p.Id
    WHERE p.IsDeleted = 0
    GROUP BY p.Id
    HAVING COUNT(ph.Id) = 0
),
photo_tags AS (
    SELECT * FROM (VALUES
        (0, 'stadium,sport'),
        (1, 'football,pitch'),
        (2, 'tennis,court'),
        (3, 'basketball,court'),
        (4, 'soccer,goal'),
        (5, 'grass,field,outdoor'),
        (6, 'athletic,track'),
        (7, 'sport,park')
    ) AS t(Idx, Tag)
)
INSERT INTO ParkPhotos (ParkId, Url, IsMain, OrderIndex, CreatedAt, UpdatedAt)
SELECT
    p.Id,
    CONCAT(
        'https://loremflickr.com/1600/900/',
        t.Tag,
        '?lock=', (p.Id * 100 + t.Idx)
    ),
    CASE WHEN t.Idx = 0 THEN 1 ELSE 0 END,
    t.Idx,
    @now,
    @now
FROM parks_to_seed p
CROSS JOIN photo_tags t;

-- Quick check: rows-per-park count and main-photo URL.
SELECT
    p.Id, p.Name,
    COUNT(ph.Id)                          AS PhotoCount,
    MAX(CASE WHEN ph.IsMain = 1 THEN ph.Url END) AS MainUrl
FROM Parks p
LEFT JOIN ParkPhotos ph ON ph.ParkId = p.Id
WHERE p.IsDeleted = 0
GROUP BY p.Id, p.Name
ORDER BY p.Id;
