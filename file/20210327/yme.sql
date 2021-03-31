
ALTER TABLE `track` ADD COLUMN `relation` int(10) UNSIGNED NULL COMMENT '并联关系(主从关系-主填写从的轨道ID)' AFTER `area`;

update `track` set relation = 11 where id = 10;
update `track` set relation = 13 where id = 12;
update `track` set relation = 15 where id = 14;
update `track` set relation = 17 where id = 16;
update `track` set relation = 19 where id = 18;
update `track` set relation = 21 where id = 20;
update `track` set relation = 23 where id = 22;
update `track` set relation = 25 where id = 24;

update config_tilelifter set work_type = 3 where id in (1,2,3,6,7);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (70, 3, 'TileMultipleLastTrackInTrans', '砖机并联轨道作业，轨道被占用', NULL, b'0', '砖机并联轨道作业，轨道被占用', NULL, NULL, NULL, NULL);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (100, 3, 'FerryNoLocation', '摆渡车失去位置信息', NULL, NULL, '摆渡车失去位置信息，请检查设备', NULL, NULL, NULL, NULL);