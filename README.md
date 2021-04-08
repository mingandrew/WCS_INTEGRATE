# WCS_INTEGRATE
 整合 2.0

#################################
分支：

main - 整合/新协议/新流程

YME - 英迈尔

TEST - 抽屉弹框（暂时）
###################################


-- 2021-04-08
-- 设定满砖数量的下限和上限
INSERT INTO `wcs_yme_yh`.`diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (121, 11, 'FullTrackLowerLimit', '满砖库存数量下限', 20, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `wcs_yme_yh`.`diction_dtl`(`id`, `diction_id`, `code`, `name`, `int_value`, `bool_value`, `string_value`, `double_value`, `uint_value`, `order`, `updatetime`) VALUES (122, 11, 'FullTrackUpperLimit', '满砖库存数量上限', 28, NULL, NULL, NULL, NULL, NULL, NULL);