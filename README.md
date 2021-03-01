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
