/*
 Navicat Premium Data Transfer

 Source Server         : TEST
 Source Server Type    : MySQL
 Source Server Version : 80016
 Source Host           : localhost:3306
 Source Schema         : 2.0wcs

 Target Server Type    : MySQL
 Target Server Version : 80016
 File Encoding         : 65001

 Date: 25/01/2021 18:50:11
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for diction
-- ----------------------------
DROP TABLE IF EXISTS `diction`;
CREATE TABLE `diction`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '类型',
  `valuetype` tinyint(3) UNSIGNED NULL DEFAULT NULL,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `isadd` bit(1) NULL DEFAULT NULL COMMENT '是否添加',
  `isedit` bit(1) NULL DEFAULT NULL COMMENT '是否编辑',
  `isdelete` bit(1) NULL DEFAULT NULL COMMENT '是否可以删除',
  `authorizelevel` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '权限等级',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `type_idx`(`type`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 12 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of diction
-- ----------------------------
INSERT INTO `diction` VALUES (1, 4, 4, '序列号生成', b'0', b'0', b'0', 1);
INSERT INTO `diction` VALUES (2, 0, 1, '任务开关', b'1', b'1', b'0', 1);
INSERT INTO `diction` VALUES (3, 0, 2, '警告信息', b'1', b'1', b'0', 1);
INSERT INTO `diction` VALUES (4, 0, 0, '存放时间', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (5, 0, 0, '安全距离', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (6, 0, 0, '版本信息', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (7, 0, 0, '转产差值', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (8, 0, 1, '配置开关', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (9, 0, 0, '等级字典', b'1', b'1', b'0', 1);
INSERT INTO `diction` VALUES (10, 0, 3, '单位转换', b'1', b'1', b'0', 1);

-- ----------------------------
-- Table structure for diction_dtl
-- ----------------------------
DROP TABLE IF EXISTS `diction_dtl`;
CREATE TABLE `diction_dtl`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `diction_id` int(11) UNSIGNED NULL DEFAULT NULL,
  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '编码',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `int_value` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '整型',
  `bool_value` bit(1) NULL DEFAULT NULL COMMENT 'bool类型',
  `string_value` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '字符串类型',
  `double_value` double(6, 3) UNSIGNED NULL DEFAULT NULL COMMENT '浮点类型',
  `uint_value` int(11) UNSIGNED NULL DEFAULT NULL,
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `updatetime` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `dic_id_fk`(`diction_id`) USING BTREE,
  CONSTRAINT `dic_id_fk` FOREIGN KEY (`diction_id`) REFERENCES `diction` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 111 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of diction_dtl
-- ----------------------------
INSERT INTO `diction_dtl` VALUES (1, 1, 'NewStockId', '生成库存ID', NULL, NULL, '', NULL, 1, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (2, 1, 'NewTranId', '生成交易ID', NULL, NULL, '', NULL, 1, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (3, 1, 'NewWarnId', '生成警告ID', NULL, NULL, '', NULL, 1, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (4, 1, 'NewGoodId', '生成品种ID', NULL, NULL, '', NULL, 1, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (11, 2, 'Area1Down', '1号线下砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (12, 2, 'Area1Up', '1号线上砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (13, 2, 'Area1Sort', '1号线倒库', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (14, 2, 'Area2Down', '2号线下砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (15, 2, 'Area2Up', '2号线上砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (16, 2, 'Area2Sort', '2号线倒库', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (17, 2, 'Area3Down', '3号线下砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (18, 2, 'Area3Up', '3号线上砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (19, 2, 'Area3Sort', '3号线倒库', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (20, 2, 'Area4Down', '4号线下砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (21, 2, 'Area4Up', '4号线上砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (22, 2, 'Area4Sort', '4号线倒库', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (23, 2, 'Area5Down', '5号线下砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (24, 2, 'Area5Up', '5号线上砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (25, 2, 'Area5Sort', '5号线倒库', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (40, 9, 'GoodLevel', '全捡混砖', 0, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (41, 9, 'GoodLevel', '优等品', 1, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (42, 9, 'GoodLevel', '一级品', 2, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (43, 9, 'GoodLevel', '二级品', 3, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (44, 9, 'GoodLevel', '合格品', 4, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (50, 4, 'MinStockTime', '最小存放时间(小时)', 0, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (51, 5, 'FerryAvoidNumber', '摆渡车(轨道数)', 3, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (52, 6, 'PDA_INIT_VERSION', 'PDA基础字典版本', 9, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (53, 6, 'PDA_GOOD_VERSION', 'PDA规格字典版本', 9, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (54, 7, 'TileLifterShiftCount', '下砖机转产差值(层数)', 99, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (55, 8, 'UserLoginFunction', 'PDA登陆功能开关', NULL, b'1', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (56, 10, 'Pulse2MM', '1脉冲=毫米', NULL, NULL, '', 17.360, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (57, 10, 'Pulse2CM', '1脉冲=厘米', NULL, NULL, '', 1.736, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (100, 3, 'WarningA1X0', '阅读器掉线', NULL, NULL, '阅读器掉线', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (101, 3, 'WarningA1X1', '急停触发', NULL, NULL, '急停触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (102, 3, 'WarningA1X2', '码盘故障', NULL, NULL, '码盘故障', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (103, 3, 'WarningA1X3', '前防撞触发', NULL, NULL, '前防撞触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (104, 3, 'WarningA1X4', '后防撞触发', NULL, NULL, '后防撞触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (105, 3, 'WarningA1X5', '下砖摆渡位置未设置', NULL, NULL, '下砖摆渡位置未设置', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (106, 3, 'WarningA1X6', '上砖摆渡位置未设置', NULL, NULL, '上砖摆渡位置未设置', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (107, 3, 'WarningA1X7', '摆渡位置设置异常', NULL, NULL, '摆渡位置设置异常', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (108, 3, 'WarningA2X0', '校验轨道号发现错误', NULL, NULL, '校验轨道号发现错误', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (109, 3, 'WarningA2X1', '前进存砖定位光电异常', NULL, NULL, '前进存砖定位光电异常', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (110, 3, 'WarningA2X2', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (111, 3, 'WarningA2X3', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (112, 3, 'WarningA2X4', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (113, 3, 'WarningA2X5', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (114, 3, 'WarningA2X6', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (115, 3, 'WarningA2X7', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (116, 3, 'WarningA3X0', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (117, 3, 'WarningA3X1', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (118, 3, 'WarningA3X2', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (119, 3, 'WarningA3X3', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (120, 3, 'WarningA3X4', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (121, 3, 'WarningA3X5', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (122, 3, 'WarningA3X6', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (123, 3, 'WarningA3X7', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (124, 3, 'WarningA4X0', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (125, 3, 'WarningA4X1', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (126, 3, 'WarningA4X2', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (127, 3, 'WarningA4X3', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (128, 3, 'WarningA4X4', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (129, 3, 'WarningA4X5', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (130, 3, 'WarningA4X6', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (131, 3, 'WarningA4X7', '暂未配置', NULL, NULL, '暂未配置报警信息', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (200, 3, 'DeviceOffline', '设备离线', NULL, NULL, '设备离线', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (201, 3, 'TrackFullButNoneStock', '轨道满砖但没库存', NULL, NULL, '轨道满砖但没库存', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (202, 3, 'CarrierLoadSortTask', '小车倒库中但是小车有货', NULL, NULL, '小车倒库中但是小车有货', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (203, 3, 'CarrierLoadNotSortTask', '小车倒库中任务清除', NULL, NULL, '小车倒库中任务清除', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (204, 3, 'TileNoneStrategy', '砖机没有设置策略', NULL, NULL, '砖机没有设置策略', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (205, 3, 'CarrierFullSignalFullNotOnStoreTrack', '小车满砖信号不在储砖轨道', NULL, NULL, '小车满砖信号不在储砖轨道', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (206, 3, 'CarrierGiveMissTrack', '小车前进放货没有扫到地标', NULL, NULL, '前进放砖没扫到地标,请手动下降放货，移回轨道头', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (207, 3, 'DownTileHaveNotTrackToStore', '砖机找不到空闲轨道存放', NULL, NULL, '砖机找不到合适轨道存砖', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (208, 3, 'UpTileHaveNotStockToOut', '砖机找不到库存出库', NULL, NULL, '砖机找不到合适库存上砖', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (209, 3, 'TrackEarlyFull', '轨道提前满砖报警', NULL, NULL, '轨道提前满砖警告', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (210, 3, 'UpTileHaveNoTrackToOut', '砖机找不到有砖轨道上砖', NULL, NULL, '砖机找不到合适轨道上砖', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (211, 3, 'CarrierLoadNeedTakeCare', '小车没任务，有货需要手动处理', NULL, NULL, '小车没任务但有货，需要手动处理', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (212, 3, 'HaveOtherCarrierInSortTrack', '有别的小车在倒库轨道，倒库车已经停止', NULL, NULL, '有别的小车在倒库轨道，倒库小车已经停止', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (213, 3, 'CarrierSortButStop', '倒库小车任务终止，需要手动发送倒库', NULL, NULL, '倒库小车任务终止，需要手动发送倒库', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (214, 3, 'TileMixLastTrackInTrans', '砖机混砖作业，轨道被占用', NULL, NULL, '砖机混砖作业，轨道被占用', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (215, 3, 'TileGoodsIsZero', '砖机工位品种反馈异常', NULL, NULL, '砖机工位品种反馈异常', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (216, 3, 'TileGoodsIsNull', '砖机工位品种没有配置', NULL, NULL, '砖机工位品种没有配置', NULL, NULL, NULL, NULL);

SET FOREIGN_KEY_CHECKS = 1;
