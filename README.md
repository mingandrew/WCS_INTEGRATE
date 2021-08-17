# WCS_INTEGRATE
 ```
 分支说明
 
 StepLogInfo:罗斯福，瑞郎，格莱利，金利源，新瑞景，华陶，景业，英迈尔，朗格，科豪
 
 Version-2.1: 新版设备通讯协议
 
 main: 整合/新协议/新流程【无步骤信息】
 
 YME: 英迈尔
 
 TEST: 抽屉弹框（暂时）
 
 ```



## 2021.01.26：新增报警
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (217, 3, 'TransHaveNotTheGiveTrack', '任务进行中没有发现合适的轨道卸砖', NULL, NULL, '任务中没有合适轨道卸砖', NULL, NULL, NULL, NULL);
```



## 2021.01.29：新增交管ID

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (5, 1, 'NewTrafficCtlId', '生成交管ID', NULL, NULL, '', NULL, 1, NULL, NULL);
```



## 2021.02.01：更新工位品种报警
```mysql
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'TileGoodsIsZero', `name` = '砖机工位品种反馈异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '砖机工位品种反馈异常，尝试使用PC当前品种', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 215;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'TileGoodsIsNull', `name` = '砖机工位品种没有配置', `int_value` = NULL, `bool_value` = NULL, `string_value` = '砖机工位品种没有配置，尝试使用PC当前品种', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 216;
```



## 2021.03.01：更新报警

```mysql
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X0', `name` = '校验轨道号发现错误', `int_value` = NULL, `bool_value` = NULL, `string_value` = '校验轨道号发现错误', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 108;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X1', `name` = '前进存砖定位光电异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '前进存砖定位光电异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 109;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X2', `name` = '下降到位信号异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '下降到位信号异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 110;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X3', `name` = '小车检测到无砖', `int_value` = NULL, `bool_value` = NULL, `string_value` = '小车检测到无砖', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 111;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X4', `name` = '顶升超时，上升到位信号异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '顶升超时，上升到位信号异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 112;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X5', `name` = '下降超时，下降到位信号异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '下降超时，下降到位信号异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 113;
```



## 2021.03.15：更新字典提示
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (218, 3, 'UpTilePreGoodNotSet', '上砖机未选预设品种，未能自动转产', NULL, NULL, '上砖机未选预设品种，未能自动转产', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (219, 3, 'DeviceSortRunOutTrack', '运输车倒库没有扫到定位点冲出轨道', NULL, NULL, '运输车倒库没有扫到定位点冲出轨道', NULL, NULL, NULL, NULL);
```



## 2021.03.30：更新模拟，看板字段

```mysql
ALTER TABLE `ferry_pos` ADD COLUMN `old_ferry_pos` int(11) NULL DEFAULT NULL COMMENT '旧的设置坐标';
ALTER TABLE `stock_log` ADD COLUMN `use` bit(1) NULL COMMENT '数据处理标志';
ALTER TABLE `config_ferry` ADD COLUMN `sim_left_site` smallint(5) UNSIGNED NULL COMMENT '模拟初始化左测对上轨道';
ALTER TABLE `config_ferry` ADD COLUMN `sim_right_site` smallint(5) UNSIGNED NULL COMMENT '模拟初始化右测对上轨道';
ALTER TABLE `config_ferry` DROP COLUMN `sim_init_point`;
ALTER TABLE `stock` ADD COLUMN `last_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '储砖轨道ID';
```



## 2021.03.31：新增摆渡车丢失位置报警

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (220, 3, 'FerryNoLocation', '摆渡车失去位置信息', NULL, NULL, '摆渡车失去位置信息，为安全起见已停止所有任务及指令的执行，待恢复位置信息后再继续作业，请检查设备！', NULL, NULL, NULL, NULL);
```



## 2021.04.06：新增分配失败报警

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (221, 3, 'FailAllocateCarrier', '运输车分配失败', NULL, NULL, '运输车分配失败', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (222, 3, 'FailAllocateFerry', '分配摆渡车失败', NULL, NULL, '分配摆渡车失败', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (223, 3, 'UpTileEmptyNeedAndNoBack', '任务中上砖机没了需求且小车无轨可回', NULL, NULL, '任务中上砖机没了需求且小车无轨可回', NULL, NULL, NULL, NULL);
```



## 2021.04.07：更新倒库报警

```mysql
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA3X4', `name` = '倒库异常，倒库空砖', `int_value` = NULL, `bool_value` = NULL, `string_value` = '倒库异常，倒库空砖', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 120;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA3X5', `name` = '取砖异常，取砖定位光电异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '取砖异常，取砖定位光电异常', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 121;
```



## 2021.04.07：新增报警信息

```mysql
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
```



## 2021.04.09：更新运输车报警提示

```mysql
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
```



## 2021.04.09：摆渡车报警信息

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (180, 3, 'WarningF_A1X0', '码盘故障', NULL, NULL, '码盘故障，请尝试手动复位设备，消除报警', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (181, 3, 'WarningF_A1X1', '急停触发', NULL, NULL, '急停触发，急停开关是否误触发？是否有异常认为打开急停开关？', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (182, 3, 'WarningF_A1X2', '下转侧对位接近开关异常', NULL, NULL, '下转侧对位接近开关异常，检查光电是否误触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (183, 3, 'WarningF_A1X3', '上砖侧对位接近开关异常', NULL, NULL, '上砖侧对位接近开关异常，检查光电是否误触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (184, 3, 'WarningF_A1X4', '前进防撞触发', NULL, NULL, '前进防撞触发，防撞光电亮黄绿灯时，请检查设备前方半米内是否有障碍物', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (185, 3, 'WarningF_A1X5', '后退防撞触发', NULL, NULL, '后退防撞触发，防撞光电亮黄绿灯时，请检查设备后方半米内是否有障碍物', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (186, 3, 'WarningF_A1X6', '暂未配置摆渡A1X6', NULL, NULL, '暂未配置摆渡报警信息A1X6', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (187, 3, 'WarningF_A1X7', '暂未配置摆渡A1X7', NULL, NULL, '暂未配置摆渡报警信息A1X7', NULL, NULL, NULL, NULL);
```

## 2021.04.13：摆渡车目的位置没有对位坐标报警

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (224, 3, 'FerryTargetUnconfigured', '摆渡车目的位置没有对位坐标值', '摆渡车目的位置没有对位坐标值，请操作重新对一次轨道位置');
```



## 2021.04.13：开关[无缝上摆渡，转产信号，备用机切换]

```mysql
sql：
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (61, 8, 'SeamlessMoveToFerry', '开关-无缝上摆渡', NULL, b'0', '', NULL, NULL, NULL, null);

UPDATE `diction_dtl` SET `name` = '开关-砖机需转产信号' WHERE `id` = 59;
UPDATE `diction_dtl` SET `name` = '开关-备用砖机自动转换' WHERE `id` = 60;
```



## 2021.04.13：倒库的同时上砖，上砖分割点接力倒库

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (62, 8, 'UpTaskIgnoreSortTask', '开关-允许倒库时可以上砖', NULL, b'0', '', NULL, NULL, NULL, NULL);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (63, 8, 'UseUpSplitPoint', '开关-启用上砖侧分割点坐标逻辑', NULL, b'0', '', NULL, NULL, NULL, NULL);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (64, 8, 'CannotUseUpSplitStock', '开关-限制直接使用上砖侧分割点后的库存', NULL, b'0', '', NULL, NULL, NULL, NULL);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) 
VALUES (65, 8, 'EnableDiagnose', '开关-启用分析服务', NULL, b'0', '', NULL, NULL, NULL, NULL);
```



## 2021.04.27：开关启用砖机的-满砖信号

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (66, 8, 'UseTileFullSign', '开关-启用砖机的-满砖信号', NULL, b'0', '', NULL, NULL, NULL, NULL);
```



## 2021.04.28：开关启用运输车交管摆渡车，新增报警，分析开关

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name` , `bool_value`) VALUES (69, 8, 'EnableCarrierTraffic', '开关-启用运输车交管摆渡车', b'0');

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (225, 3, 'CarrierFreeButMoveInFerry', '运输车空闲状态(停止/指令完成)但处于上下摆渡中' , '运输车空闲状态(停止/指令完成)但处于上下摆渡中，无法解锁相应摆渡车');

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `string_value`) VALUES (226, 3, 'CarrierFreeInFerryButLocErr', '运输车空闲状态(停止/指令完成)在摆渡上，但当前轨道有误' , '运输车空闲状态(停止/指令完成)在摆渡上，但当前轨道有误，无法解锁相应摆渡车');

新增字典分析服务开关sql:

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (67, 8, 'EnableSortDiagnose', '开关-启用倒库分析服务', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (68, 8, 'EnableMoveCarDiagnose', '开关-启用移车分析服务', NULL, b'0', '', NULL, NULL, NULL, NULL);
```



## 2021.05.12：报警提示，启用极限混砖开关

```mysql
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'DownTileHaveNotTrackToStore', `name` = '砖机找不到空闲轨道存放', `int_value` = NULL, `bool_value` = NULL, `string_value` = '砖机找不到合适轨道（品种及状态允许且无任务锁定）存砖', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 207;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'UpTileHaveNoTrackToOut', `name` = '砖机找不到有砖轨道上砖', `int_value` = NULL, `bool_value` = NULL, `string_value` = '砖机找不到合适轨道（品种及状态允许且无任务锁定）上砖', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 210;
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'TransHaveNotTheGiveTrack', `name` = '任务进行中没有发现合适的轨道卸砖', `int_value` = NULL, `bool_value` = NULL, `string_value` = '任务中没有合适轨道（品种及状态允许且无任务锁定）卸砖', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 217;

新增开关启用下砖入库极限混砖sql:
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (70, 8, 'EnableLimitAllocate', '开关-启用下砖入库极限混砖', NULL, b'0', NULL, NULL, NULL, NULL, NULL);
```



## 2021.05.12：开关-接力限制倒库数量【在线路上配置】

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (71, 8, 'UpSortUseMaxNumber', '开关-接力限制倒库数量', NULL, b'0', NULL, NULL, NULL, NULL, NULL);

线路添加-接力限制倒库数量

ALTER TABLE `line` ADD COLUMN `max_upsort_num` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '接力限制倒库数量';
```



## 2021.05.17： 轨道脉冲设置

```mysql
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (34, '轨道脉冲配置', 0, 'TrackSetPoint', NULL, 'DarkPrimaryBrush', 'ConfigGeometry', 'TrackSetPointCtl', 'PC轨道脉冲配置');
```



## 2021.05.18： 开关的报警信息

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (227, 3, 'DownTaskSwitchClosed', '【下砖任务开关】关闭', NULL, NULL, '【下砖任务开关】已关闭', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (228, 3, 'UpTaskSwitchClosed', '【上砖任务开关】关闭', NULL, NULL, '【上砖任务开关】已关闭', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (229, 3, 'SortTaskSwitchClosed', '【倒库任务开关】关闭', NULL, NULL, '【倒库任务开关】已关闭', NULL, NULL, NULL, NULL);
```



## 2021.05.19：关于上下砖时间限制作业轨道
```mysql
新增字段 sql:
ALTER TABLE `config_tilelifter` ADD COLUMN `non_work_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '砖机不作业轨道' AFTER `last_track_id`;

新增开关 sql:
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (72, 8, 'EnableStockTimeForUp', '开关-启用上砖库存时间限制', NULL, b'0', '品种库存最早时间在入库侧-停止上砖且报警', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (73, 8, 'EnableStockTimeForDown', '开关-启用下砖库存时间限制', NULL, b'0', '不得连续下满同一条轨道-仅剩最后一条轨道时停止下砖且报警', NULL, NULL, NULL, NULL);

新增报警 sql:
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (230, 3, 'TheEarliestStockInDown', '最早的库存在下砖入库侧轨道', NULL, NULL, '以先进先出为原则，发现最早的库存在下砖入库侧轨道，暂无法上砖', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (231, 3, 'PreventTimeConflict', '不能连续下砖但仅剩最后一条轨道', NULL, NULL, '不允许同品种下砖连续下满同一条轨道，需变更轨道下砖，防止时间冲突', NULL, NULL, NULL, NULL);
```


## 2021.05.21：关于下砖时按轨道顺序存放
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (74, 8, 'EnableDownTrackOrder', '开关-启用下砖顺序存放', NULL, b'0', '下砖时按轨道顺序存放', NULL, NULL, NULL, NULL);
```


## 2021.05.22：更新上砖机的最近轨道为0，让上砖机按照时间顺序上砖
```mysql
update config_tilelifter set last_track_id = 0 where work_mode = 1;
```


## 2021.05.28：移除运输车配置库存ID外键
```mysql
ALTER TABLE `config_carrier` DROP FOREIGN KEY `carrier_stock_id_fk`;
```

## 2021.06.01：有备用砖机的项目
> 有备用砖机的项目，需要提前配置好摆渡车的轨道，因为启用备用砖机时，摆渡车的分配轨道不变，能去的轨道不会改！！！！！！！

## 2021.06.09：更新倒库完成后入库还有库存报警
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (232, 3, 'SortFinishButDownExistStock', '倒库指令已完成，入库还有库存', NULL, NULL, '运输车倒库完成后入库轨道还有库存，请在核实并修改入库轨道的库存之后，1.如果需要继续倒库，请手动给运输车发倒库任务，2.如果不需要继续倒库，请取消当前轨道的倒库任务和修改轨道状态为有砖/空砖', NULL, NULL, NULL, NULL);
```

## 2021.06.11：更新倒库空砖报警修改
```mysql
UPDATE `diction_dtl` SET `string_value` = '倒库空砖，请检测光电是否正常和轨道库存是否正确' WHERE `id` = 114;
```


## 2021.06.11：线路开关

```mysql
ALTER TABLE `line` ADD COLUMN `onoff_up` bit(1) NULL COMMENT '上砖开关' AFTER `max_upsort_num`;
ALTER TABLE `line` ADD COLUMN `onoff_down` bit(1) NULL COMMENT '下砖开关' AFTER `onoff_up`;
ALTER TABLE `line` ADD COLUMN `onoff_sort` bit(1) NULL COMMENT '倒库开关' AFTER `onoff_down`;
ALTER TABLE `line` ADD COLUMN `line_type` tinyint(3) UNSIGNED NULL COMMENT '线类型：0窑后 1包装前' AFTER `onoff_sort`;
```




## 2021.06.15：报警添加线路字段，等级字段

```mysql
ALTER TABLE `warning` ADD COLUMN `line_id` smallint(5) NULL COMMENT '线路ID' AFTER `area_id`;
ALTER TABLE `warning` ADD COLUMN `level` TINYINT(3) UNSIGNED NULL COMMENT '等级';
```




## 2021.06.15：报警字典添加等级

```mysql
ALTER TABLE `diction_dtl` ADD COLUMN `level` tinyint(3) UNSIGNED NULL COMMENT '等级';
```



## 2021.06.16：添加字典控制是否能使用平板清除按钮
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (75, 8, 'AllowClearTask', '开关-是否能使用平板清除按钮', NULL, NULL, '是否能使用平板清除按钮', NULL, NULL, NULL, NULL, NULL); 
```




## 2021.06.17：添加开关-允许出入倒库时可以上砖
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (76, 8, 'UpTaskIgnoreInoutSortTask', '开关-允许出入倒库时可以上砖', NULL, NULL, '开关-允许出入倒库时可以上砖', NULL, NULL, NULL, NULL, NULL);
```




## 2021.06.19：后退取砖取空报警
```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (233, 3, 'GetStockButNull', '取砖指令完成后，没有取到砖', NULL, NULL, '运输车取砖空砖，请检查取砖光电是否异常不亮，(如运输车在储砖轨道请核实轨道库存)，最后给运输车发终止指令', NULL, NULL, NULL, NULL, NULL);

UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA2X3', `name` = '小车检测到无砖', `int_value` = NULL, `bool_value` = NULL, `string_value` = '后退取砖失败，请检查有砖光电是否异常不亮，（如运输车在储砖轨道请核实轨道库存），最后给运输车发终止指令', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL, `level` = NULL WHERE `id` = 111;


```

## 2021.06.21：更新小车报警 A3X6
```mysql
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'WarningA3X6', `name` = '取砖异常，存砖定位光电异常', `int_value` = NULL, `bool_value` = NULL, `string_value` = '取砖异常，存砖定位光电异常，检查存砖定位光电是否误触发', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL WHERE `id` = 122;
```




## 2021.06.22：反抛任务的启用

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (77, 8, 'EnableSecondUpTask', '开关-启用反抛任务', NULL, b'0', '开关-启用反抛任务', NULL, NULL, NULL, NULL, NULL);
```



## 2021.06.22：添加报警-不执行反抛任务

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (234, 3, 'Warning34', '【反抛未执行】，等待上砖机工位空砖', NULL, NULL, '【反抛未执行】，等待上砖机工位空砖', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (235, 3, 'Warning35', '【反抛未执行】，等待上砖侧库存里无反抛任务的品种可上，或者上砖机转产', NULL, NULL, '【反抛未执行】，等待上砖侧库存里无反抛任务的品种可上，或者上砖机转产', NULL, NULL, NULL, NULL, NULL);
```



## 2021.06.29：添加报警-流程超时报警

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (236, 3, 'Warning36', '【流程超时】', NULL, NULL, '【流程超时】', NULL, NULL, NULL, NULL, NULL);
```



## 2021-06-30：新增字段，用于设定上下砖侧的运输车的数量

> ### **如果某一个数量为0，则不会自动将运输车在出库轨道入库轨道来回调用**

```mysql
ALTER TABLE `area` ADD COLUMN `up_car_count` tinyint(3) UNSIGNED NULL COMMENT '上砖运输车的最少数量限定' AFTER `full_qty`;
ALTER TABLE `area` ADD COLUMN `down_car_count` tinyint(3) UNSIGNED NULL COMMENT '下砖运输车的最少数量限定' AFTER `up_car_count`;
```



## 2021-06-30：新增线的入库轨道满砖数量上限 - 设置为0，则不需要限制它的数量

```mysql
ALTER TABLE `line` ADD COLUMN `full_qty` tinyint(3) UNSIGNED NULL COMMENT '入库轨道满砖上限' AFTER `line_type`;
```



## 2021.06.30：摆渡车对位码盘报警

```mysql
UPDATE `diction_dtl` SET `name` = '对轨码盘偏差超出允许范围，判断是前侧接近开关误触', 
`string_value` = '对轨码盘偏差超出允许范围，判断是前侧接近开关误触，请检查接近开关是否误触，若无误触情况，请操作回原点进行复位' WHERE `id` = 186;

UPDATE `diction_dtl` SET `name` = '对轨码盘偏差超出允许范围，判断是后侧接近开关误触', 
`string_value` = '对轨码盘偏差超出允许范围，判断是后侧接近开关误触，请检查接近开关是否误触，若无误触情况，请操作回原点进行复位' WHERE `id` = 187;

```



## 2021.06.30：新增流程超时设置

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (250, 4, 'StepOverTime', '除【倒库中】，其他流程的超时时间（秒）', 600, NULL, NULL, NULL, NULL, NULL, '2021-06-30 08:44:37', NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (251, 4, 'SortingStockStepOverTime', '倒库中流程的超时时间（秒）', 7200, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
```



## 2021.06.30：更新运输车报警信息：码盘故障

```mysql
UPDATE `diction_dtl` SET `string_value` = '码盘故障，请尝试手动发送终止指令，消除报警' WHERE `id` = 102;
```



## 2021.06.30：库存整理-任务细单信息

```mysql
CREATE TABLE `stock_trans_dtl` (
  `dtl_id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `dtl_p_id` int(11) unsigned DEFAULT NULL COMMENT '父任务ID',
  `dtl_trans_id` int(11) unsigned DEFAULT NULL COMMENT '当前执行任务ID',
  `dtl_area_id` int(11) unsigned DEFAULT NULL COMMENT '区域ID',
  `dtl_line_id` smallint(3) unsigned DEFAULT NULL COMMENT '线路ID',
  `dtl_type` tinyint(3) unsigned DEFAULT NULL COMMENT '类型',
  `dtl_good_id` int(11) unsigned DEFAULT NULL COMMENT '品种ID',
  `dtl_take_track_id` int(11) unsigned DEFAULT NULL COMMENT '取货ID',
  `dtl_give_track_id` int(11) unsigned DEFAULT NULL COMMENT '卸货ID',
  `dtl_status` tinyint(3) DEFAULT NULL COMMENT '细单状态',
  `dtl_all_qty` smallint(3) unsigned DEFAULT NULL COMMENT '全部数量',
  `dtl_left_qty` smallint(3) unsigned DEFAULT NULL COMMENT '剩余数量',
  `dtl_finish` bit(1) DEFAULT NULL COMMENT '完成',
  PRIMARY KEY (`dtl_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (10, 1, 'NewTranDtlId', '生成任务细单ID', NULL, b'0', '', NULL, 1, NULL, NULL);

INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (35, '库存整理', 0, 'OrganizeTrack', NULL, 'DarkPrimaryBrush', 'ConfigGeometry', 'OrganizeTrackCtl', '库存整理');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (NULL, '库存整理', b'0', 8, 35, 3, NULL);
```



## 2021.06.30：库存整理-平板 

```mysql
INSERT INTO `wcs_module`(`id`, `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES (36, '库存整理', 1, 'RFSORTSOTCK', 'com.keda.wcsfixplatformapp.screen.rforganize.RfOrganizeTransMainScreen', NULL, 'othersetting.png', NULL, '平板-库存整理');

INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES ( 1, '库存整理', b'0', NULL, 36, 17, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '库存整理', b'0', NULL, 36, 17, b'1');

```



## 2021.07.03：上砖设定品种数量

**新增上砖数量为0的报警**

```mysql
 INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (237, 3, 'Warning37', '当前品种设定的上砖数量为零', NULL, NULL, '当前品种设定的上砖数量为零', NULL, NULL, NULL, NULL, NULL);
```

 

 **新增砖机当前品种数量和预设品种数量字段**

 ```mysql
 ALTER TABLE `config_tilelifter` ADD COLUMN `now_good_qty` int(11) UNSIGNED NULL COMMENT '当前品种数量' AFTER `level`;
 ALTER TABLE `config_tilelifter` ADD COLUMN `pre_good_qty` int(11) UNSIGNED NULL COMMENT '预约品种数量' AFTER `now_good_qty`;
 ALTER TABLE `config_tilelifter` ADD COLUMN `now_good_all` bit(1) NULL COMMENT '当前使用全部库存-不限制库存数量' AFTER `pre_good_qty`;
 ALTER TABLE `config_tilelifter` ADD COLUMN `pre_good_all` bit(1) NULL COMMENT '预约使用全部库存-不限制库存数量' AFTER `now_good_all`;
 
 UPDATE `config_tilelifter` SET `now_good_all` = b'1';
 UPDATE `config_tilelifter` SET `pre_good_all` = b'1';
 ```



## 2021.07.05：更新上砖设定品种数量的报警信息

```mysql
UPDATE `diction_dtl` SET `diction_id` = 3, `code` = 'Warning37', `name` = '当前品种设定的上砖数量为零', `int_value` = NULL, `bool_value` = NULL, `string_value` = '当前品种设定的上砖数量为零,请执行转产操作', `double_value` = NULL, `uint_value` = NULL, `order` = NULL, `updatetime` = NULL, `level` = NULL WHERE `id` = 237;

```

## 2021.07.07：更新品种数量限制

```mysql
INSERT INTO `diction`(`id`, `type`, `valuetype`, `name`, `isadd`, `isedit`, `isdelete`, `authorizelevel`) VALUES (11, 0, 0, '品种列表', b'0', b'1', b'0', 1);

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (238, 11, 'GoodsListLimit', '品种列表数量上限', 100, NULL, NULL, NULL, NULL, NULL, '2021-07-06 10:29:17', NULL);
```

## 2021.07.08：库存统计加上线的信息

```mysql
CREATE OR REPLACE ALGORITHM = UNDEFINED DEFINER = `root`@`localhost` SQL SECURITY DEFINER VIEW `stock_sum` AS SELECT
	`t`.`goods_id` AS `goods_id`,
	`t`.`track_id` AS `track_id`,
	min( `t`.`produce_time` ) AS `produce_time`,
	count( `t`.`id` ) AS `count`,
	sum( `t`.`pieces` ) AS `pieces`,
	sum( `t`.`stack` ) AS `stack`,
	`t`.`area` AS `area`,
	`t`.`track_type` AS `track_type`,
	(select line from track where id = t.track_id) as `line`

FROM
	`stock` `t` 
WHERE
	(
	`t`.`track_type` IN ( 2, 3, 4 )) 
GROUP BY
	`t`.`track_id`,
	`t`.`goods_id` 
ORDER BY
	`t`.`area`,
	`t`.`goods_id`,
	`produce_time`,
	`t`.`track_id`;
```



## 2021.07.08 [ V2.1 ]：新增运输车报警

### [ V2.1 ]  新增运输车报警
```mysql

INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (239 3, 'CarrierIsInResetWork', '运输车初始化/寻点指令中，已暂停相关作业，请确认操作完成-发送终止指令', NULL, NULL, '运输车初始化/寻点指令中，已暂停相关作业，请确认操作完成-发送终止指令', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (240, 3, 'CarrierNoLocation', '运输车失去位置信息', NULL, NULL, '运输车失去位置信息，为安全起见已停止所有任务及指令的执行，待恢复位置信息后再继续作业，请检查设备进行位置初始化操作！', NULL, NULL, NULL, NULL, NULL);
```
### [ V2.1 ]  新增轨道属性字段，确认轨道存取方向

```mysql
ALTER TABLE `track` ADD COLUMN `is_give_back` bit(1) NULL COMMENT '是否入库 后退存砖' AFTER `up_split_point`;
ALTER TABLE `track` ADD COLUMN `is_take_forward` bit(1) NULL COMMENT '是否出库 前进取砖' AFTER `is_give_back`;
```

### [ V2.1 ]  运输车新版报警更新
```mysql
UPDATE `diction_dtl` SET `name` = '急停触发', `string_value` = '急停触发，急停开关是否误触发？是否有异常认为打开急停开关？' WHERE `id` = 100;
UPDATE `diction_dtl` SET `name` = '开始定位点范围内未取到砖', `string_value` = '开始定位点范围内未取到砖：1.取砖定位光电无触发；2.库存脉冲数据与实际不符' WHERE `id` = 101;
UPDATE `diction_dtl` SET `name` = '未到取砖开始定位点范围，取砖定位光电触发', `string_value` = '未到取砖开始定位点范围，取砖定位光电触发：1.取砖定位误触发；2.库存脉冲实际与实际不符' WHERE `id` = 102;
UPDATE `diction_dtl` SET `name` = '小车有砖光电未触发', `string_value` = '小车取砖过程中有砖光电无触发' WHERE `id` = 103;
UPDATE `diction_dtl` SET `name` = '前防撞触发', `string_value` = '前防撞触发，防撞光电亮黄绿灯时，请检查设备前方半米内是否有障碍物' WHERE `id` = 104;
UPDATE `diction_dtl` SET `name` = '后防撞触发', `string_value` = '后防撞触发，防撞光电亮黄绿灯时，请检查设备后方半米内是否有障碍物' WHERE `id` = 105;
UPDATE `diction_dtl` SET `name` = '下降超时', `string_value` = '下降超时，下降到位接近开关异常，检查下位接近开关' WHERE `id` = 106;
UPDATE `diction_dtl` SET `name` = '上升超时', `string_value` = '上升超时，上升到位接近开关异常，检查上位接近开关' WHERE `id` = 107;
UPDATE `diction_dtl` SET `name` = '未在上位状态', `string_value` = '未在上位状态，顶板位置异常' WHERE `id` = 108;
UPDATE `diction_dtl` SET `name` = '未在下位状态', `string_value` = '未在下位状态，顶板位置异常' WHERE `id` = 109;
UPDATE `diction_dtl` SET `name` = '未在下位前进时，载砖前进存砖定位光电保护触发', `string_value` = '未在下位前进时，载砖前进存砖定位光电保护触发：1.存砖定位光电误触发；2.库存脉冲实际与实际不符' WHERE `id` = 110;
UPDATE `diction_dtl` SET `name` = '未在下位后退时，载砖后退存砖定位光电保护触发', `string_value` = '未在下位后退时，载砖后退存砖定位光电保护触发：1.存砖定位光电误触发；2.库存脉冲实际与实际不符' WHERE `id` = 111;
UPDATE `diction_dtl` SET `name` = '上下摆渡时，超出保护范围位置', `string_value` = '上下摆渡时，超出保护范围位置：小车轨道站点接近开关未触发' WHERE `id` = 112;
UPDATE `diction_dtl` SET `name` = '开始定位点=0', `string_value` = '开始定位点=0' WHERE `id` = 113;
UPDATE `diction_dtl` SET `name` = '结束定位点=0', `string_value` = '结束定位点=0' WHERE `id` = 114;
UPDATE `diction_dtl` SET `name` = '当前码盘坐标值=0', `string_value` = '当前码盘坐标值=0：码盘脉冲数值异常' WHERE `id` = 115;
UPDATE `diction_dtl` SET `name` = '1xx轨道码盘数值_复位点=0', `string_value` = '1xx轨道码盘数值_复位点=0' WHERE `id` = 116;
UPDATE `diction_dtl` SET `name` = '1xx轨道码盘数值_定位点=0', `string_value` = '1xx轨道码盘数值_定位点=0' WHERE `id` = 117;
UPDATE `diction_dtl` SET `name` = '2xx轨道码盘数值_定位点=0', `string_value` = '2xx轨道码盘数值_定位点=0' WHERE `id` = 118;
UPDATE `diction_dtl` SET `name` = '3xx轨道下砖侧码盘数值_定位点=0', `string_value` = '3xx轨道下砖侧码盘数值_定位点=0' WHERE `id` = 119;
UPDATE `diction_dtl` SET `name` = '3xx轨道下砖侧码盘数值_复位点=0', `string_value` = '3xx轨道下砖侧码盘数值_复位点=0' WHERE `id` = 120;
UPDATE `diction_dtl` SET `name` = '3xx轨道中间码盘数值_复位点=0', `string_value` = '3xx轨道中间码盘数值_复位点=0' WHERE `id` = 121;
UPDATE `diction_dtl` SET `name` = '3xx轨道上砖侧码盘数值_复位点=0', `string_value` = '3xx轨道上砖侧码盘数值_复位点=0' WHERE `id` = 122;
UPDATE `diction_dtl` SET `name` = '3xx轨道上砖侧码盘数值_定位点=0', `string_value` = '3xx轨道上砖侧码盘数值_定位点=0' WHERE `id` = 123;
UPDATE `diction_dtl` SET `name` = '4xx轨道码盘数值_定位点=0', `string_value` = '4xx轨道码盘数值_定位点=0' WHERE `id` = 124;
UPDATE `diction_dtl` SET `name` = '5xx轨道码盘数值_定位点=0', `string_value` = '5xx轨道码盘数值_定位点=0' WHERE `id` = 125;
UPDATE `diction_dtl` SET `name` = '5xx轨道码盘数值_复位点=0', `string_value` = '5xx轨道码盘数值_复位点=0' WHERE `id` = 126;
UPDATE `diction_dtl` SET `name` = '码盘丢转故障', `string_value` = '码盘丢转故障，小车移动过程中码盘接近开关超过5秒无信号：1.检查是否撞车；2.检查刹车是否能正常打开；3.检查模拟量输出模块电压输出是否大于1.5V；4.检查码盘接近开关；检查无误时终止可复位' WHERE `id` = 127;
```



## 2021.07.12  [ V2.0 ]  优化串联下砖机的任务生成顺序
```mysql
ALTER TABLE `tilelifterneed` ADD COLUMN `prior` smallint unsigned NULL COMMENT '优先级' AFTER `area_id`;
```

## 2021.07.14 [ V2.1 ]  新增区域设备轨道对应关系-砖机配置轨道上下砖作业区分
```mysql
ALTER TABLE `area_device_track` ADD COLUMN `can_up` bit(1) NULL COMMENT '是否用于上砖' AFTER `prior`;
ALTER TABLE `area_device_track` ADD COLUMN `can_down` bit(1) NULL COMMENT '是否用于下砖' AFTER `can_up`;
UPDATE area_device_track set can_up = true, can_down = true;
```


## 2021.07.14 [ V2.0 ]  整理字典顺序 
```mysql
UPDATE `diction_dtl` SET `id` = 78 WHERE `id` = 250;
UPDATE `diction_dtl` SET `id` = 79 WHERE `id` = 251;
```



## 2021.07.16 [ V2.0 ] 添加运输车报警

```mysql
UPDATE `diction_dtl` SET `name` = '任务被中断时，尚未升降到位，运输车将不再接受新任务。请先手动将运输车升降到位，再进行其它操作', `string_value` = '任务被中断时，尚未升降到位，运输车将不再接受新任务。请先手动将运输车升降到位，再进行其它操作' WHERE `code` = 'WarningA3X7';
```



## 2021.07.26 [ V2.0 ] 添加轨道倒库属性

```mysql
ALTER TABLE `track` 
ADD COLUMN `sort_able` bit(1) NULL COMMENT '是否可以倒库' AFTER `up_split_point`,
ADD COLUMN `sort_level` tinyint(3) UNSIGNED NULL COMMENT '倒库优先级' AFTER `sort_able`;
```



## 2021.07.30 [ V2.1 ]   增加最后生产时间

```mysql
ALTER VIEW `stock_sum` AS
SELECT s.area AS 'area', t.line AS 'line', s.goods_id AS 'goods_id',
			 s.track_id AS 'track_id', t.type AS 'track_type', t.type2 AS 'track_type2', 
			 COUNT(s.id) AS 'count', SUM(s.pieces) AS 'pieces', SUM(s.stack) AS 'stack',
			 MIN(s.produce_time) AS 'produce_time', MAX(s.produce_time) AS 'last_produce_time'
  FROM stock s
  LEFT JOIN track t ON s.track_id = t.id
 WHERE s.track_type IN (2,3,4)
 GROUP BY s.track_id, s.goods_id
 ORDER BY s.area, s.goods_id, 'produce_time', s.track_id;
 

```

## 2021.07.30 [V2.0] 新增移车任务的轨道可用于下砖任务的开关

```mysql
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (80, 8, 'EnableMoveToDown', '开关-启用移车时可下砖', NULL, b'0', '开关-启用移车时可下砖', NULL, NULL, NULL, NULL, NULL);
```



## 2021.07.30 [V2.0] 谨慎更新！！！ 设置所有的任务开关打开，然后删除了电脑和平板上的任务开关界面，需要提前和现场人员沟通好

```mysql
-- UPDATE line SET onoff_up = 1, onoff_down = 1, onoff_sort = 1;

-- DELETE FROM	wcs_menu_dtl WHERE	module_id IN (SELECT id FROM wcs_module WHERE `key` = "AreaSwitch");
```



## 2021.08.02 [V2.1] 更新菜单平板配置

请确认新加的模块module_id能对应上

```mysql
INSERT INTO `wcs_module`( `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES ( '位置初始化', 1, 'DeviceReset', 'com.keda.wcsfixplatformapp.screen.rfinitdevice.RfInitDeviceMainScreen', NULL, 'shiftcar.png', NULL, '平板-设备初始化');
INSERT INTO `wcs_module`( `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES ( '调试模块', 1, 'DEBUGMODULE', 'com.keda.wcsfixplatformapp.screen.rfdebugmodule.RfDebugModuleScreen', NULL, 'updowndev.png', NULL, '平板-调试模块');
INSERT INTO `wcs_module`( `name`, `type`, `key`, `entity`, `brush`, `geometry`, `winctlname`, `memo`) VALUES ( '手动任务', 1, 'MANUALTASK', 'com.keda.wcsfixplatformapp.screen.rfmanualtask.RfManualTaskScreen', NULL, 'othersetting.png', NULL, '平板-手动任务');

DELETE FROM `wcs_menu_dtl` WHERE `module_id` = 22 AND `menu_id` = 1 AND `rf` = 1;
DELETE FROM `wcs_menu_dtl` WHERE `module_id` = 23 AND `menu_id` = 1 AND `rf` = 1;
DELETE FROM `wcs_menu_dtl` WHERE `module_id` = 24 AND `menu_id` = 1 AND `rf` = 1;
DELETE FROM `wcs_menu_dtl` WHERE `module_id` = 25 AND `menu_id` = 1 AND `rf` = 1;
DELETE FROM `wcs_menu_dtl` WHERE `module_id` = 32 AND `menu_id` = 1 AND `rf` = 1;

UPDATE `wcs_menu_dtl` SET  `order` = 1 WHERE `menu_id` = 1 and `module_id` = 27 and `rf` = b'1';
UPDATE `wcs_menu_dtl` SET  `order` = 2 WHERE `menu_id` = 1 and `module_id` = 19 and `rf` = b'1';
UPDATE `wcs_menu_dtl` SET  `order` = 3 WHERE `menu_id` = 1 and `module_id` = 21 and `rf` = b'1';
UPDATE `wcs_menu_dtl` SET  `order` = 4 WHERE `menu_id` = 1 and `module_id` = 20 and `rf` = b'1';
UPDATE `wcs_menu_dtl` SET  `order` = 5 WHERE `menu_id` = 1 and `module_id` = 26 and `rf` = b'1';

INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (1, '位置初始化', b'0', 0, 37, 6, b'1');

INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '任务开关', b'0', 0, 22, 1, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '任务信息', b'0', 0, 27, 2, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '品种设置', b'0', 0, 19, 3, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '砖机规格', b'0', 0, 21, 4, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '轨道设置', b'0', 0, 20, 5, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '轨道库存', b'0', 0, 26, 6, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '位置初始化', b'0', 0, 37, 7, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '调试模块', NULL, 0, 38, 8, b'1');
INSERT INTO `wcs_menu_dtl`(`menu_id`, `name`, `folder`, `folder_id`, `module_id`, `order`, `rf`) VALUES (2, '手动任务', b'0', 0, 39, 9, b'1');



```



## 2021.08.16 [V2.1] 更新小车新报警信息

```mysql
UPDATE `diction_dtl` SET `name` = '当前位置不能靠光电前进取砖', `string_value` = '未在3xx轨道内，不能执行前进取砖位置值65535（靠光电取砖），请先将小车移至3xx储砖轨道' WHERE `id` = 128;

UPDATE `diction_dtl` SET `name` = '当前位置不能靠光电后退取砖', `string_value` = '未在3xx轨道内，不能执行后退取砖位置值1（靠光电取砖），请先将小车移至3xx储砖轨道' WHERE `id` = 129;
```

​	

## 2021.08.17 [2.1] 新增库存等级字段
```mysql
ALTER TABLE `stock` ADD COLUMN `level` tinyint unsigned NULL COMMENT '砖机等级/窑位' AFTER `last_track_id`;	

ALTER TABLE `stock_trans` ADD COLUMN `level` int unsigned NULL COMMENT '砖机等级/窑位' AFTER `cancel`;

ALTER TABLE `goods` DROP COLUMN `level`;

ALTER TABLE `config_tilelifter` ADD COLUMN `level_type` tinyint unsigned NULL COMMENT '等级类型：0等级，1窑位' AFTER `pre_good_all`;
ALTER TABLE `config_tilelifter` ADD COLUMN `syn_tile_list` varchar(10) NULL COMMENT '同步转产砖机id（用#隔开）' AFTER `level_type`;
ALTER TABLE `config_tilelifter` ADD COLUMN `pre_level` tinyint(0) UNSIGNED NULL DEFAULT NULL COMMENT '上砖机专用--预设的品种等级' AFTER `level`;


UPDATE `diction_dtl` SET `int_value` = 1 WHERE `code` = 'GoodLevel' AND `name` = '全捡混砖';
UPDATE `diction_dtl` SET `int_value` = 2 WHERE `code` = 'GoodLevel' AND `name` = '优等品';
UPDATE `diction_dtl` SET `int_value` = 3 WHERE `code` = 'GoodLevel' AND `name` = '一级品';
UPDATE `diction_dtl` SET `int_value` = 4 WHERE `code` = 'GoodLevel' AND `name` = '二级品';
UPDATE `diction_dtl` SET `int_value` = 5 WHERE `code` = 'GoodLevel' AND `name` = '合格品';


INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (45, 9, 'TileSite', '窑位1', 6, NULL, '窑位1', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (46, 9, 'TileSite', '窑位2', 7, NULL, '窑位2', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (47, 9, 'TileSite', '窑位3', 8, NULL, '窑位3', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (48, 9, 'TileSite', '窑位4', 9, NULL, '窑位4', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`, `level`) VALUES (49, 9, 'TileSite', '窑位5', 10, NULL, '窑位5', NULL, NULL, NULL, NULL, NULL);
```


## 2021.08.17 [2.1] 更新统计视图-使用等级来区分库存信息
​```mysql

ALTER VIEW `stock_sum` AS
SELECT s.area AS 'area', t.line AS 'line', s.goods_id AS 'goods_id',
			 s.track_id AS 'track_id', t.type AS 'track_type', t.type2 AS 'track_type2', 
			 COUNT(s.id) AS 'count', SUM(s.pieces) AS 'pieces', SUM(s.stack) AS 'stack',
			 MIN(s.produce_time) AS 'produce_time', MAX(s.produce_time) AS 'last_produce_time',
			 `s`.`level` AS `sum_level` 
  FROM stock s
  LEFT JOIN track t ON s.track_id = t.id
 WHERE s.track_type IN (2,3,4)
 GROUP BY s.track_id, s.goods_id, s.`level` 
 ORDER BY s.area, s.goods_id, s.`level`, 'produce_time', s.track_id;
```