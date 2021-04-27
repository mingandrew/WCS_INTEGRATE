# WCS_INTEGRATE
 整合 2.0

#################################
分支：

main - 整合/新协议/新流程

YME - 英迈尔

TEST - 抽屉弹框（暂时）
###################################



2021.01.26
新增报警sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (217, 3, 'TransHaveNotTheGiveTrack', '任务进行中没有发现合适的轨道卸砖', NULL, NULL, '任务中没有合适轨道卸砖', NULL, NULL, NULL, NULL);



2021.01.29
新增交管sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (5, 1, 'NewTrafficCtlId', '生成交管ID', NULL, NULL, '', NULL, 1, NULL, NULL);



2021.02.01
更新工位品种报警sql：
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'TileGoodsIsZero', `name` = '砖机工位品种反馈异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '砖机工位品种反馈异常，尝试使用PC当前品种', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 215;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'TileGoodsIsNull', `name` = '砖机工位品种没有配置', `int_value` = NULL, `bool_value` = NULL, `string_value` = '砖机工位品种没有配置，尝试使用PC当前品种', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 216;




2021.03.01
更新报警sql：
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X0', `name` = '校验轨道号发现错误', `int_value` = NULL, `bool_value` = NULL, `string_value` = '校验轨道号发现错误', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 108;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X1', `name` = '前进存砖定位光电异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '前进存砖定位光电异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 109;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X2', `name` = '下降到位信号异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '下降到位信号异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 110;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X3', `name` = '小车检测到无砖', `int_value` = NULL, `bool_value` = NULL, `string_value` = '小车检测到无砖', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 111;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X4', `name` = '顶升超时，上升到位信号异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '顶升超时，上升到位信号异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 112;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X5', `name` = '下降超时，下降到位信号异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '下降超时，下降到位信号异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 113;

2021.03.15
更新字典提升：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (218, 3, 'UpTilePreGoodNotSet', '上砖机未选预设品种，未能自动转产', NULL, NULL, '上砖机未选预设品种，未能自动转产', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (219, 3, 'DeviceSortRunOutTrack', '运输车倒库没有扫到定位点冲出轨道', NULL, NULL, '运输车倒库没有扫到定位点冲出轨道', NULL, NULL, NULL, NULL);


2021.03.30
更新模拟，看板字段sql：
ALTER TABLE `ferry_pos` ADD COLUMN `old_ferry_pos` int(11) NULL DEFAULT NULL COMMENT '旧的设置坐标';
ALTER TABLE `stock_log` ADD COLUMN `use` bit(1) NULL COMMENT '数据处理标志';
ALTER TABLE `config_ferry` ADD COLUMN `sim_left_site` smallint(5) UNSIGNED NULL COMMENT '模拟初始化左测对上轨道';
ALTER TABLE `config_ferry` ADD COLUMN `sim_right_site` smallint(5) UNSIGNED NULL COMMENT '模拟初始化右测对上轨道';
ALTER TABLE `config_ferry` DROP COLUMN `sim_init_point`;
ALTER TABLE `stock` ADD COLUMN `last_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '储砖轨道ID';


2021.03.31
新增报警sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (220, 3, 'FerryNoLocation', '摆渡车失去位置信息', NULL, NULL, '摆渡车失去位置信息，为安全起见已停止所有任务及指令的执行，待恢复位置信息后再继续作业，请检查设备！', NULL, NULL, NULL, NULL);


2021.04.06
新增报警sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (221, 3, 'FailAllocateCarrier', '运输车分配失败', NULL, NULL, '运输车分配失败', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (222, 3, 'FailAllocateFerry', '分配摆渡车失败', NULL, NULL, '分配摆渡车失败', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (223, 3, 'UpTileEmptyNeedAndNoBack', '任务中上砖机没了需求且小车无轨可回', NULL, NULL, '任务中上砖机没了需求且小车无轨可回', NULL, NULL, NULL, NULL);



2021.04.07
更新报警sql：
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA3X4', `name` = '倒库异常，倒库空砖', `int_value` = NULL, `bool_value` = NULL, `string_value` = '倒库异常，倒库空砖', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 120;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA3X5', `name` = '取砖异常，取砖定位光电异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '取砖异常，取砖定位光电异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 121;



2021.04.07
新增报警sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	132, 3, 'WarningA5X0' , '暂未配置A5X0' , '暂未配置报警信息A5X0');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	133, 3, 'WarningA5X1' , '暂未配置A5X1' , '暂未配置报警信息A5X1');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	134, 3, 'WarningA5X2' , '暂未配置A5X2' , '暂未配置报警信息A5X2');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	135, 3, 'WarningA5X3' , '暂未配置A5X3' , '暂未配置报警信息A5X3');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	136, 3, 'WarningA5X4' , '暂未配置A5X4' , '暂未配置报警信息A5X4');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	137, 3, 'WarningA5X5' , '暂未配置A5X5' , '暂未配置报警信息A5X5');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	138, 3, 'WarningA5X6' , '暂未配置A5X6' , '暂未配置报警信息A5X6');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	139, 3, 'WarningA5X7' , '暂未配置A5X7' , '暂未配置报警信息A5X7');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	140, 3, 'WarningA6X0' , '暂未配置A6X0' , '暂未配置报警信息A6X0');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	141, 3, 'WarningA6X1' , '暂未配置A6X1' , '暂未配置报警信息A6X1');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	142, 3, 'WarningA6X2' , '暂未配置A6X2' , '暂未配置报警信息A6X2');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	143, 3, 'WarningA6X3' , '暂未配置A6X3' , '暂未配置报警信息A6X3');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	144, 3, 'WarningA6X4' , '暂未配置A6X4' , '暂未配置报警信息A6X4');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	145, 3, 'WarningA6X5' , '暂未配置A6X5' , '暂未配置报警信息A6X5');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	146, 3, 'WarningA6X6' , '暂未配置A6X6' , '暂未配置报警信息A6X6');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	147, 3, 'WarningA6X7' , '暂未配置A6X7' , '暂未配置报警信息A6X7');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	148, 3, 'WarningA7X0' , '暂未配置A7X0' , '暂未配置报警信息A7X0');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	149, 3, 'WarningA7X1' , '暂未配置A7X1' , '暂未配置报警信息A7X1');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	150, 3, 'WarningA7X2' , '暂未配置A7X2' , '暂未配置报警信息A7X2');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	151, 3, 'WarningA7X3' , '暂未配置A7X3' , '暂未配置报警信息A7X3');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	152, 3, 'WarningA7X4' , '暂未配置A7X4' , '暂未配置报警信息A7X4');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	153, 3, 'WarningA7X5' , '暂未配置A7X5' , '暂未配置报警信息A7X5');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	154, 3, 'WarningA7X6' , '暂未配置A7X6' , '暂未配置报警信息A7X6');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	155, 3, 'WarningA7X7' , '暂未配置A7X7' , '暂未配置报警信息A7X7');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	156, 3, 'WarningA8X0' , '暂未配置A8X0' , '暂未配置报警信息A8X0');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	157, 3, 'WarningA8X1' , '暂未配置A8X1' , '暂未配置报警信息A8X1');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	158, 3, 'WarningA8X2' , '暂未配置A8X2' , '暂未配置报警信息A8X2');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	159, 3, 'WarningA8X3' , '暂未配置A8X3' , '暂未配置报警信息A8X3');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	160, 3, 'WarningA8X4' , '暂未配置A8X4' , '暂未配置报警信息A8X4');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	161, 3, 'WarningA8X5' , '暂未配置A8X5' , '暂未配置报警信息A8X5');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	162, 3, 'WarningA8X6' , '暂未配置A8X6' , '暂未配置报警信息A8X6');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	163, 3, 'WarningA8X7' , '暂未配置A8X7' , '暂未配置报警信息A8X7');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	164, 3, 'WarningA9X0' , '暂未配置A9X0' , '暂未配置报警信息A9X0');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	165, 3, 'WarningA9X1' , '暂未配置A9X1' , '暂未配置报警信息A9X1');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	166, 3, 'WarningA9X2' , '暂未配置A9X2' , '暂未配置报警信息A9X2');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	167, 3, 'WarningA9X3' , '暂未配置A9X3' , '暂未配置报警信息A9X3');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	168, 3, 'WarningA9X4' , '暂未配置A9X4' , '暂未配置报警信息A9X4');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	169, 3, 'WarningA9X5' , '暂未配置A9X5' , '暂未配置报警信息A9X5');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	170, 3, 'WarningA9X6' , '暂未配置A9X6' , '暂未配置报警信息A9X6');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	171, 3, 'WarningA9X7' , '暂未配置A9X7' , '暂未配置报警信息A9X7');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	172, 3, 'WarningA10X0' , '暂未配置A10X0' , '暂未配置报警信息A10X0');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	173, 3, 'WarningA10X1' , '暂未配置A10X1' , '暂未配置报警信息A10X1');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	174, 3, 'WarningA10X2' , '暂未配置A10X2' , '暂未配置报警信息A10X2');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	175, 3, 'WarningA10X3' , '暂未配置A10X3' , '暂未配置报警信息A10X3');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	176, 3, 'WarningA10X4' , '暂未配置A10X4' , '暂未配置报警信息A10X4');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	177, 3, 'WarningA10X5' , '暂未配置A10X5' , '暂未配置报警信息A10X5');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	178, 3, 'WarningA10X6' , '暂未配置A10X6' , '暂未配置报警信息A10X6');
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (	179, 3, 'WarningA10X7' , '暂未配置A10X7' , '暂未配置报警信息A10X7');


2021.04.09
更新报警sql：
UPDATE `diction_dtl` SET `string_value` = '阅读器掉线，阅读器状态灯为红色时，检查连接线是否松动。' WHERE `id` = 100;
UPDATE `diction_dtl` SET `string_value` = '急停触发，急停开关是否误触发？是否有异常认为打开急停开关？' WHERE `id` = 101;
UPDATE `diction_dtl` SET `string_value` = '码盘故障，请尝试手动复位设备，消除报警' WHERE `id` = 102;
UPDATE `diction_dtl` SET `string_value` = '前防撞触发，防撞光电亮黄绿灯时，请检查设备前方半米内是否有障碍物' WHERE `id` = 103;
UPDATE `diction_dtl` SET `string_value` = '后防撞触发，防撞光电亮黄绿灯时，请检查设备后方半米内是否有障碍物' WHERE `id` = 104;
UPDATE `diction_dtl` SET `string_value` = '下砖摆渡位置未设置，请在调度系统重新设置下转侧摆渡车复位点坐标值为1000' WHERE `id` = 105;
UPDATE `diction_dtl` SET `string_value` = '上砖摆渡位置未设置，请在调度系统重新设置上转侧摆渡车复位点坐标值为实际测量值' WHERE `id` = 106;
UPDATE `diction_dtl` SET `string_value` = '摆渡位置设置异常，下摆渡复位点坐标与上摆渡复位点坐标差值小于1000，请重新设置才能正常使用' WHERE `id` = 107;
UPDATE `diction_dtl` SET `string_value` = '校验轨道号发现错误，分配的轨道号与实际进入轨道号不符合，请检查任务分配轨道与实际轨道号是否一致' WHERE `id` = 108;
UPDATE `diction_dtl` SET `string_value` = '前进存砖定位光电异常，检查光电是否误触发' WHERE `id` = 109;
UPDATE `diction_dtl` SET `string_value` = '下降到位信号异常，检查下位接近开关有没有信号' WHERE `id` = 110;
UPDATE `diction_dtl` SET `string_value` = '后退取砖空砖，调度系统自动删除库存信息，恢复自动作业' WHERE `id` = 111;
UPDATE `diction_dtl` SET `string_value` = '顶升超时，上升到位接近开关异常，检查上位接近开关' WHERE `id` = 112;
UPDATE `diction_dtl` SET `string_value` = '下降超时，下降到位接近开关异常，检查下位接近开关' WHERE `id` = 113;
UPDATE `diction_dtl` SET `string_value` = '倒库空砖，调度系统自动删除库存信息，恢复自动作业' WHERE `id` = 114;
UPDATE `diction_dtl` SET `string_value` = '倒库异常自保护，前进存砖定位光电异常，检查前进存砖定位光电是否误触发' WHERE `id` = 115;
UPDATE `diction_dtl` SET `string_value` = '倒库异常自保护，有砖光电异常，检查有砖光电是否误触发' WHERE `id` = 116;
UPDATE `diction_dtl` SET `string_value` = '倒库异常自保护，后退取砖定位光电异常，检查后退取砖定位光电是否误触发' WHERE `id` = 117;
UPDATE `diction_dtl` SET `string_value` = '前进极限触发保护，设备停止后，调度系统可正常调度' WHERE `id` = 118;
UPDATE `diction_dtl` SET `string_value` = '后退极限触发保护，设备停止后，调度系统可正常调度' WHERE `id` = 119;
UPDATE `diction_dtl` SET `string_value` = '倒库异常，倒库空砖，检查光电是否误触发' WHERE `id` = 120;
UPDATE `diction_dtl` SET `string_value` = '取砖异常，取砖定位光电异常，检查取砖定位光电是否误触发' WHERE `id` = 121;


2021.04.09
新增报警sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (180, 3, 'WarningF_A1X0', '码盘故障', NULL, NULL, '码盘故障，请尝试手动复位设备，消除报警', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (181, 3, 'WarningF_A1X1', '急停触发', NULL, NULL, '急停触发，急停开关是否误触发？是否有异常认为打开急停开关？', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (182, 3, 'WarningF_A1X2', '下转侧对位接近开关异常', NULL, NULL, '下转侧对位接近开关异常，检查光电是否误触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (183, 3, 'WarningF_A1X3', '上砖侧对位接近开关异常', NULL, NULL, '上砖侧对位接近开关异常，检查光电是否误触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (184, 3, 'WarningF_A1X4', '前进防撞触发', NULL, NULL, '前进防撞触发，防撞光电亮黄绿灯时，请检查设备前方半米内是否有障碍物', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (185, 3, 'WarningF_A1X5', '后退防撞触发', NULL, NULL, '后退防撞触发，防撞光电亮黄绿灯时，请检查设备后方半米内是否有障碍物', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (186, 3, 'WarningF_A1X6', '暂未配置摆渡A1X6', NULL, NULL, '暂未配置摆渡报警信息A1X6', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (187, 3, 'WarningF_A1X7', '暂未配置摆渡A1X7', NULL, NULL, '暂未配置摆渡报警信息A1X7', NULL, NULL, NULL, NULL);


2021.04.13
新增报警sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (224, 3, 'FerryTargetUnconfigured', '摆渡车目的位置没有对位坐标值', '摆渡车目的位置没有对位坐标值，请操作重新对一次轨道位置');

2021.04.13
新增开关sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (61, 8, 'SeamlessMoveToFerry', '开关-无缝上摆渡', NULL, b'0', '', NULL, NULL, NULL, null);

UPDATE `diction_dtl` SET `name` = '开关-砖机需转产信号' WHERE `id` = 59;
UPDATE `diction_dtl` SET `name` = '开关-备用砖机自动转换' WHERE `id` = 60;

2021.04.13	倒库的同时上砖，上砖分割点接力倒库
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (62, 8, 'UpTaskIgnoreSortTask', '开关-允许倒库时可以上砖', NULL, b'0', '', NULL, NULL, NULL, NULL);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (63, 8, 'UseUpSplitPoint', '开关-启用上砖侧分割点坐标逻辑', NULL, b'0', '', NULL, NULL, NULL, NULL);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (64, 8, 'CannotUseUpSplitStock', '开关-限制直接使用上砖侧分割点后的库存', NULL, b'0', '', NULL, NULL, NULL, NULL);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (65, 8, 'EnableDiagnose', '开关-启用分析服务', NULL, b'0', '', NULL, NULL, NULL, NULL);




2021.04.27
新增开关启用砖机的-满砖信号sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (66, 8, 'UseTileFullSign', '开关-启用砖机的-满砖信号', NULL, b'0', '', NULL, NULL, NULL, NULL);












