SELECT
	t.track_id,
	t.produce_time,
	t.pos,
	t.pos_type,
	t.location,
	t.location_cal ,
	cast(t.location as signed)-cast(t.location_cal as signed) as "差距 "
FROM
	stock t 
	where
	1=1
 and	t.track_type in (2,3,4) -- 入库轨道
ORDER BY
	t.track_id,
	t.pos