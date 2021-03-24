-- 启用模拟

update device set ip = '127.0.0.1';
update device set `port` = 2001 where type in(0,1);
update device set `port` = 2002 where type in(2,3);
update device set `port` = 2003 where type = 4 ;


-- 恢复原来的数据
update device d set ip = (select c.ip from device_copy1 c where c.id = d.id);
update device set `port` = 2000 ;