/*
 Navicat Premium Data Transfer

 Source Server         : TEST
 Source Server Type    : MySQL
 Source Server Version : 80016
 Source Host           : localhost:3306
 Source Schema         : 2021_wcs_oml_bzq

 Target Server Type    : MySQL
 Target Server Version : 80016
 File Encoding         : 65001

 Date: 25/06/2021 19:00:03
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for area
-- ----------------------------
DROP TABLE IF EXISTS `area`;
CREATE TABLE `area`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '区域名称',
  `enable` bit(1) NULL DEFAULT NULL COMMENT '区域可用',
  `devautorun` bit(1) NULL DEFAULT NULL COMMENT '设备自启动',
  `memo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `c_sorttask` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '倒库任务数量',
  `carriertype` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '运输车类型',
  `full_qty` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '轨道未达到满砖警告数',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area
-- ----------------------------
INSERT INTO `area` VALUES (1, '包装前#', b'1', b'1', '包装前', 1, 0, 1);

-- ----------------------------
-- Table structure for area_device
-- ----------------------------
DROP TABLE IF EXISTS `area_device`;
CREATE TABLE `area_device`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
  `device_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '设备ID',
  `dev_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '设备类型1',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `at_tile_id_fk`(`device_id`) USING BTREE,
  INDEX `at_area_id_fk`(`area_id`) USING BTREE,
  CONSTRAINT `at_area_id_fk` FOREIGN KEY (`area_id`) REFERENCES `area` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `at_tile_id_fk` FOREIGN KEY (`device_id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域-设备-关系表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_device
-- ----------------------------
INSERT INTO `area_device` VALUES (1, 1, 1, 6);
INSERT INTO `area_device` VALUES (2, 1, 2, 1);
INSERT INTO `area_device` VALUES (3, 1, 3, 2);
INSERT INTO `area_device` VALUES (4, 1, 4, 4);
INSERT INTO `area_device` VALUES (5, 1, 5, 4);

-- ----------------------------
-- Table structure for area_device_track
-- ----------------------------
DROP TABLE IF EXISTS `area_device_track`;
CREATE TABLE `area_device_track`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
  `device_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '设备ID',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '轨道ID',
  `prior` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '优先级',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `adt_area_id_fk`(`area_id`) USING BTREE,
  INDEX `adt_device_id_fk`(`device_id`) USING BTREE,
  INDEX `adt_track_id_fk`(`track_id`) USING BTREE,
  CONSTRAINT `adt_area_id_fk` FOREIGN KEY (`area_id`) REFERENCES `area` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `adt_device_id_fk` FOREIGN KEY (`device_id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `adt_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 21 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域-设备-轨道-关系表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_device_track
-- ----------------------------
INSERT INTO `area_device_track` VALUES (1, 1, 1, 5, 1);
INSERT INTO `area_device_track` VALUES (2, 1, 1, 6, 2);
INSERT INTO `area_device_track` VALUES (3, 1, 1, 7, 3);
INSERT INTO `area_device_track` VALUES (4, 1, 1, 8, 4);
INSERT INTO `area_device_track` VALUES (5, 1, 1, 9, 5);
INSERT INTO `area_device_track` VALUES (6, 1, 1, 10, 6);
INSERT INTO `area_device_track` VALUES (7, 1, 1, 11, 7);
INSERT INTO `area_device_track` VALUES (8, 1, 1, 12, 8);
INSERT INTO `area_device_track` VALUES (9, 1, 1, 13, 9);
INSERT INTO `area_device_track` VALUES (10, 1, 1, 14, 10);
INSERT INTO `area_device_track` VALUES (11, 1, 2, 5, 1);
INSERT INTO `area_device_track` VALUES (12, 1, 2, 6, 2);
INSERT INTO `area_device_track` VALUES (13, 1, 2, 7, 3);
INSERT INTO `area_device_track` VALUES (14, 1, 2, 8, 4);
INSERT INTO `area_device_track` VALUES (15, 1, 2, 9, 5);
INSERT INTO `area_device_track` VALUES (16, 1, 2, 10, 6);
INSERT INTO `area_device_track` VALUES (17, 1, 2, 11, 7);
INSERT INTO `area_device_track` VALUES (18, 1, 2, 12, 8);
INSERT INTO `area_device_track` VALUES (19, 1, 2, 13, 9);
INSERT INTO `area_device_track` VALUES (20, 1, 2, 14, 10);

-- ----------------------------
-- Table structure for area_track
-- ----------------------------
DROP TABLE IF EXISTS `area_track`;
CREATE TABLE `area_track`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '轨道ID',
  `track_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '轨道类型',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `atra_area_id_fk`(`area_id`) USING BTREE,
  INDEX `atra_track_id_fk`(`track_id`) USING BTREE,
  CONSTRAINT `atra_area_id_fk` FOREIGN KEY (`area_id`) REFERENCES `area` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `atra_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 17 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域-轨道-关系表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_track
-- ----------------------------
INSERT INTO `area_track` VALUES (1, 1, 1, 1);
INSERT INTO `area_track` VALUES (2, 1, 2, 1);
INSERT INTO `area_track` VALUES (3, 1, 3, 1);
INSERT INTO `area_track` VALUES (4, 1, 4, 1);
INSERT INTO `area_track` VALUES (5, 1, 5, 4);
INSERT INTO `area_track` VALUES (6, 1, 6, 4);
INSERT INTO `area_track` VALUES (7, 1, 7, 4);
INSERT INTO `area_track` VALUES (8, 1, 8, 4);
INSERT INTO `area_track` VALUES (9, 1, 9, 4);
INSERT INTO `area_track` VALUES (10, 1, 10, 4);
INSERT INTO `area_track` VALUES (11, 1, 11, 4);
INSERT INTO `area_track` VALUES (12, 1, 12, 4);
INSERT INTO `area_track` VALUES (13, 1, 13, 4);
INSERT INTO `area_track` VALUES (14, 1, 14, 4);
INSERT INTO `area_track` VALUES (15, 1, 15, 5);
INSERT INTO `area_track` VALUES (16, 1, 16, 1);

-- ----------------------------
-- Table structure for carrier_pos
-- ----------------------------
DROP TABLE IF EXISTS `carrier_pos`;
CREATE TABLE `carrier_pos`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '小车复位原点ID',
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
  `track_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '复位地标',
  `track_pos` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '复位脉冲',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '运输车复位坐标表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of carrier_pos
-- ----------------------------
INSERT INTO `carrier_pos` VALUES (1, 1, 10, 1000);
INSERT INTO `carrier_pos` VALUES (2, 1, 11, 1301);
INSERT INTO `carrier_pos` VALUES (3, 1, 12, 1551);
INSERT INTO `carrier_pos` VALUES (4, 1, 50, 2023);
INSERT INTO `carrier_pos` VALUES (5, 1, 98, 2501);
INSERT INTO `carrier_pos` VALUES (6, 1, 99, 2751);
INSERT INTO `carrier_pos` VALUES (7, 1, 100, 0);

-- ----------------------------
-- Table structure for config_carrier
-- ----------------------------
DROP TABLE IF EXISTS `config_carrier`;
CREATE TABLE `config_carrier`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '运输车设备ID',
  `a_takemisstrack` bit(1) NULL DEFAULT NULL COMMENT '后退取货扫不到点',
  `a_givemisstrack` bit(1) NULL DEFAULT NULL COMMENT '前进放货扫不到点',
  `a_alert_track` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '所在轨道',
  `stock_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '库存ID',
  `length` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '运输车顶板长度（脉冲）',
  `goods_size` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '负责规格（通用车不需要填规格ID，否则用 # 隔开）',
  `sim_init_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '模拟初始脉冲',
  `sim_init_site` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '模拟初始地标',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `carrier_stock_id_index`(`stock_id`) USING BTREE,
  CONSTRAINT `carrier_id_fk` FOREIGN KEY (`id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '配置表-运输车' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config_carrier
-- ----------------------------
INSERT INTO `config_carrier` VALUES (4, b'0', b'0', 0, NULL, 196, NULL, NULL, NULL);
INSERT INTO `config_carrier` VALUES (5, b'0', b'0', 0, NULL, 196, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for config_ferry
-- ----------------------------
DROP TABLE IF EXISTS `config_ferry`;
CREATE TABLE `config_ferry`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '摆渡车设备ID',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车轨道ID',
  `track_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车轨道地标',
  `sim_left_site` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '模拟初始化左测对上轨道',
  `sim_right_site` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '模拟初始化右测对上轨道',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `ferry_track_id_index`(`track_id`) USING BTREE,
  CONSTRAINT `ferry__id_fk` FOREIGN KEY (`id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `ferry_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '配置表-摆渡车' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config_ferry
-- ----------------------------
INSERT INTO `config_ferry` VALUES (3, 15, 401, NULL, NULL);

-- ----------------------------
-- Table structure for config_tilelifter
-- ----------------------------
DROP TABLE IF EXISTS `config_tilelifter`;
CREATE TABLE `config_tilelifter`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '砖机设备ID',
  `brother_dev_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '前设备ID[兄弟砖机ID]',
  `left_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上下砖机：左轨道ID',
  `left_track_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '上下砖机：左轨道工位地标',
  `right_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上下砖机：右轨道ID',
  `right_track_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '上下砖机：右轨道工位地标',
  `strategy_in` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '下砖策略',
  `strategy_out` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '上砖策略',
  `work_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '作业类型\r\n砖机：0按规格 1按轨道',
  `last_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '砖机上次作业轨道',
  `non_work_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '砖机不作业轨道',
  `old_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上一个品种',
  `goods_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '品种ID',
  `pre_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '预设品种',
  `do_shift` bit(1) NULL DEFAULT NULL COMMENT '开启转产',
  `can_cutover` bit(1) NULL DEFAULT NULL COMMENT '可切换模式',
  `work_mode` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '作业模式\r\n0：过砖模式\r\n1：上砖模式\r\n2：下砖模式',
  `work_mode_next` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '下一个作业模式',
  `do_cutover` bit(1) NULL DEFAULT NULL COMMENT '开启切换模式',
  `can_alter` bit(1) NULL DEFAULT NULL COMMENT '是否备用',
  `alter_ids` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备用砖机id（用 , 隔开）',
  `alter_dev_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '当前备用砖机ID',
  `level` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '砖机等级',
  `good_change` bit(1) NULL DEFAULT NULL COMMENT '品种更改',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `tile_goods_id_index`(`goods_id`) USING BTREE,
  INDEX `tile_ltrack_id_index`(`left_track_id`) USING BTREE,
  INDEX `tile_rtrack_id_index`(`right_track_id`) USING BTREE,
  CONSTRAINT `tile_goods_id_fk` FOREIGN KEY (`goods_id`) REFERENCES `goods` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `tile_id_fk` FOREIGN KEY (`id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `tile_ltrack_id_fk` FOREIGN KEY (`left_track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `tile_rtrack_id_fk` FOREIGN KEY (`right_track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '配置表-砖机' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config_tilelifter
-- ----------------------------
INSERT INTO `config_tilelifter` VALUES (1, 0, 1, 1828, 2, 1828, 2, 0, 0, 0, NULL, NULL, NULL, NULL, b'0', b'1', 0, 0, b'0', b'0', '0', NULL, NULL, NULL);
INSERT INTO `config_tilelifter` VALUES (2, 0, 3, 1828, 4, 1828, 2, 0, 0, 0, NULL, NULL, NULL, NULL, b'0', b'0', 2, 0, b'0', b'0', '0', NULL, NULL, NULL);

-- ----------------------------
-- Table structure for consume_log
-- ----------------------------
DROP TABLE IF EXISTS `consume_log`;
CREATE TABLE `consume_log`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `goods_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '品种ID',
  `area` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域',
  `stack` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '垛数',
  `pieces` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '片数',
  `track_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '储砖轨道ID',
  `produce_tile_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '下砖机ID',
  `produce_time` datetime(0) NULL DEFAULT NULL COMMENT '生产时间',
  `consume_tile_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '上砖机ID',
  `consume_time` datetime(0) NULL DEFAULT NULL COMMENT '上砖消耗时间',
  `use` bit(1) NULL DEFAULT NULL COMMENT '数据处理标志',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 744 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '库存消耗记录表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of consume_log
-- ----------------------------

-- ----------------------------
-- Table structure for device
-- ----------------------------
DROP TABLE IF EXISTS `device`;
CREATE TABLE `device`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `ip` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `port` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '类型',
  `type2` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '类型二',
  `enable` bit(1) NULL DEFAULT NULL COMMENT '可用',
  `att1` tinyint(3) UNSIGNED NULL DEFAULT NULL,
  `att2` tinyint(3) UNSIGNED NULL DEFAULT NULL,
  `memo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `area` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域值用于过滤',
  `line` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '线：区域 > 线',
  `do_work` bit(1) NULL DEFAULT NULL COMMENT '开启作业',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '设备表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of device
-- ----------------------------
INSERT INTO `device` VALUES (1, '1A52', '192.168.0.36', 2000, 6, 2, b'0', 0, 0, '162', 1, 1, b'1');
INSERT INTO `device` VALUES (2, '1A51', '192.168.0.31', 2000, 1, 2, b'0', 0, 0, '161', 1, 1, b'1');
INSERT INTO `device` VALUES (3, '1B51', '192.168.0.133', 2000, 2, 0, b'0', 0, 0, '179', 1, 1, b'1');
INSERT INTO `device` VALUES (4, '1C51', '192.168.0.151', 2000, 4, 0, b'0', 0, 0, '193', 1, 1, b'0');
INSERT INTO `device` VALUES (5, '1C52', '192.168.0.152', 2000, 4, 0, b'0', 0, 0, '194', 1, 1, b'1');

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
) ENGINE = InnoDB AUTO_INCREMENT = 11 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '字典表' ROW_FORMAT = Dynamic;

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
  `string_value` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '字符串类型',
  `double_value` double(10, 3) UNSIGNED NULL DEFAULT NULL COMMENT '浮点类型',
  `uint_value` int(11) UNSIGNED NULL DEFAULT NULL,
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `updatetime` datetime(0) NULL DEFAULT NULL,
  `level` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '等级',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `dic_id_fk`(`diction_id`) USING BTREE,
  CONSTRAINT `dic_id_fk` FOREIGN KEY (`diction_id`) REFERENCES `diction` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 234 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '字典明细表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of diction_dtl
-- ----------------------------
INSERT INTO `diction_dtl` VALUES (1, 1, 'NewStockId', '生成库存ID', NULL, NULL, '', NULL, 43, NULL, '2021-06-24 08:53:31', NULL);
INSERT INTO `diction_dtl` VALUES (2, 1, 'NewTranId', '生成交易ID', NULL, NULL, '', NULL, 1, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (3, 1, 'NewWarnId', '生成警告ID', NULL, NULL, '', NULL, 71, NULL, '2021-06-25 17:52:37', NULL);
INSERT INTO `diction_dtl` VALUES (4, 1, 'NewGoodId', '生成品种ID', NULL, NULL, '', NULL, 2, NULL, '2021-06-23 14:45:40', NULL);
INSERT INTO `diction_dtl` VALUES (5, 1, 'NewTrafficCtlId', '生成交管ID', NULL, NULL, '', NULL, 1, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (40, 9, 'GoodLevel', '全捡混砖', 0, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (41, 9, 'GoodLevel', '优等品', 1, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (42, 9, 'GoodLevel', '一级品', 2, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (43, 9, 'GoodLevel', '二级品', 3, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (44, 9, 'GoodLevel', '合格品', 4, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (50, 4, 'MinStockTime', '最小存放时间(小时)', 0, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (51, 5, 'FerryAvoidNumber', '摆渡车(轨道数)', 3, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (52, 6, 'PDA_INIT_VERSION', 'PDA基础字典版本', 600, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (53, 6, 'PDA_GOOD_VERSION', 'PDA规格字典版本', 601, NULL, '', NULL, NULL, NULL, '2021-06-23 14:45:40', NULL);
INSERT INTO `diction_dtl` VALUES (54, 7, 'TileLifterShiftCount', '下砖机转产差值(层数)', 99, NULL, '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (55, 8, 'UserLoginFunction', 'PDA登陆功能开关', NULL, b'1', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (56, 10, 'Pulse2MM', '1脉冲=毫米', NULL, NULL, '', 17.360, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (57, 10, 'Pulse2CM', '1脉冲=厘米', NULL, NULL, '', 1.736, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (58, 10, 'StackPluse', '一垛计算距离', NULL, NULL, '', 220.000, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (59, 8, 'TileNeedSysShiftFunc', '开关-砖机需转产信号', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (60, 8, 'AutoBackupTileFunc', '开关-备用砖机自动转换', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (61, 8, 'SeamlessMoveToFerry', '开关-无缝上摆渡', NULL, b'0', '', NULL, NULL, NULL, '2021-06-25 17:46:52', NULL);
INSERT INTO `diction_dtl` VALUES (62, 8, 'UpTaskIgnoreSortTask', '开关-允许倒库时可以上砖', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (63, 8, 'UseUpSplitPoint', '开关-启用上砖侧分割点坐标逻辑', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (64, 8, 'CannotUseUpSplitStock', '开关-限制直接使用上砖侧分割点后的库存', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (65, 8, 'EnableDiagnose', '开关-启用分析服务', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (66, 8, 'UseTileFullSign', '开关-启用砖机的-满砖信号', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (67, 8, 'EnableSortDiagnose', '开关-启用倒库分析服务', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (68, 8, 'EnableMoveCarDiagnose', '开关-启用移车分析服务', NULL, b'0', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (69, 8, 'EnableCarrierTraffic', '开关-启用运输车交管摆渡车', NULL, b'1', NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (70, 8, 'EnableLimitAllocate', '开关-启用下砖入库极限混砖', NULL, b'0', NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (71, 8, 'UpSortUseMaxNumber', '开关-接力限制倒库数量', NULL, b'0', NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (72, 8, 'EnableStockTimeForUp', '开关-启用上砖库存时间限制', NULL, b'0', '品种库存最早时间在入库侧-停止上砖且报警', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (73, 8, 'EnableStockTimeForDown', '开关-启用下砖库存时间限制', NULL, b'0', '不得连续下满同一条轨道-仅剩最后一条轨道时停止下砖且报警', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (74, 8, 'EnableDownTrackOrder', '开关-启用下砖顺序存放', NULL, b'0', '下砖时按轨道顺序存放', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (75, 8, 'AllowClearTask', '开关-是否能使用平板清除按钮', NULL, b'0', '是否能使用平板清除按钮', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (76, 8, 'UpTaskIgnoreInoutSortTask', '开关-允许出入倒库时可以上砖', NULL, b'0', '开关-允许出入倒库时可以上砖', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (100, 3, 'WarningA1X0', '阅读器掉线', NULL, NULL, '阅读器掉线，阅读器状态灯为红色时，检查连接线是否松动。', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (101, 3, 'WarningA1X1', '急停触发', NULL, NULL, '急停触发，急停开关是否误触发？是否有异常认为打开急停开关？', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (102, 3, 'WarningA1X2', '码盘故障', NULL, NULL, '码盘故障，请尝试手动复位设备，消除报警', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (103, 3, 'WarningA1X3', '前防撞触发', NULL, NULL, '前防撞触发，防撞光电亮黄绿灯时，请检查设备前方半米内是否有障碍物', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (104, 3, 'WarningA1X4', '后防撞触发', NULL, NULL, '后防撞触发，防撞光电亮黄绿灯时，请检查设备后方半米内是否有障碍物', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (105, 3, 'WarningA1X5', '下砖摆渡位置未设置', NULL, NULL, '下砖摆渡位置未设置，请在调度系统重新设置下转侧摆渡车复位点坐标值为1000', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (106, 3, 'WarningA1X6', '上砖摆渡位置未设置', NULL, NULL, '上砖摆渡位置未设置，请在调度系统重新设置上转侧摆渡车复位点坐标值为实际测量值', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (107, 3, 'WarningA1X7', '摆渡位置设置异常', NULL, NULL, '摆渡位置设置异常，下摆渡复位点坐标与上摆渡复位点坐标差值小于1000，请重新设置才能正常使用', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (108, 3, 'WarningA2X0', '校验轨道号发现错误', NULL, NULL, '校验轨道号发现错误，分配的轨道号与实际进入轨道号不符合，请检查任务分配轨道与实际轨道号是否一致', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (109, 3, 'WarningA2X1', '前进存砖定位光电异常', NULL, NULL, '前进存砖定位光电异常，检查光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (110, 3, 'WarningA2X2', '下降到位信号异常', NULL, NULL, '下降到位信号异常，检查下位接近开关有没有信号', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (111, 3, 'WarningA2X3', '小车检测到无砖', NULL, NULL, '后退取砖失败，请检查有砖光电是否异常不亮，（如运输车在储砖轨道请核实轨道库存），最后给运输车发终止指令', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (112, 3, 'WarningA2X4', '顶升超时，上升到位信号异常', NULL, NULL, '顶升超时，上升到位接近开关异常，检查上位接近开关', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (113, 3, 'WarningA2X5', '下降超时，下降到位信号异常', NULL, NULL, '下降超时，下降到位接近开关异常，检查下位接近开关', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (114, 3, 'WarningA2X6', '运输车倒库无砖', NULL, NULL, '倒库空砖，请检测光电是否正常和轨道库存是否正确', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (115, 3, 'WarningA2X7', '倒库异常自保护，前进存砖定位光电异常', NULL, NULL, '倒库异常自保护，前进存砖定位光电异常，检查前进存砖定位光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (116, 3, 'WarningA3X0', '倒库异常自保护，有砖光电异常', NULL, NULL, '倒库异常自保护，有砖光电异常，检查有砖光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (117, 3, 'WarningA3X1', '倒库异常自保护，后退取砖定位光电异常', NULL, NULL, '倒库异常自保护，后退取砖定位光电异常，检查后退取砖定位光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (118, 3, 'WarningA3X2', '前进极限触发保护', NULL, NULL, '前进极限触发保护', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (119, 3, 'WarningA3X3', '后退极限触发保护', NULL, NULL, '后退极限触发保护', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (120, 3, 'WarningA3X4', '倒库异常，倒库空砖', NULL, NULL, '倒库异常，倒库空砖，检查光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (121, 3, 'WarningA3X5', '取砖异常，取砖定位光电异常', NULL, NULL, '取砖异常，取砖定位光电异常，检查取砖定位光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (122, 3, 'WarningA3X6', '取砖异常，存砖定位光电异常', NULL, NULL, '取砖异常，存砖定位光电异常，检查存砖定位光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (123, 3, 'WarningA3X7', '暂未配置A3X7', NULL, NULL, '暂未配置报警信息A3X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (124, 3, 'WarningA4X0', '暂未配置A4X0', NULL, NULL, '暂未配置报警信息A4X0', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (125, 3, 'WarningA4X1', '暂未配置A4X1', NULL, NULL, '暂未配置报警信息A4X1', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (126, 3, 'WarningA4X2', '暂未配置A4X2', NULL, NULL, '暂未配置报警信息A4X2', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (127, 3, 'WarningA4X3', '暂未配置A4X3', NULL, NULL, '暂未配置报警信息A4X3', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (128, 3, 'WarningA4X4', '暂未配置A4X4', NULL, NULL, '暂未配置报警信息A4X4', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (129, 3, 'WarningA4X5', '暂未配置A4X5', NULL, NULL, '暂未配置报警信息A4X5', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (130, 3, 'WarningA4X6', '暂未配置A4X6', NULL, NULL, '暂未配置报警信息A4X6', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (131, 3, 'WarningA4X7', '暂未配置A4X7', NULL, NULL, '暂未配置报警信息A4X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (132, 3, 'WarningA5X0', '暂未配置A5X0', NULL, NULL, '暂未配置报警信息A5X0', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (133, 3, 'WarningA5X1', '暂未配置A5X1', NULL, NULL, '暂未配置报警信息A5X1', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (134, 3, 'WarningA5X2', '暂未配置A5X2', NULL, NULL, '暂未配置报警信息A5X2', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (135, 3, 'WarningA5X3', '暂未配置A5X3', NULL, NULL, '暂未配置报警信息A5X3', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (136, 3, 'WarningA5X4', '暂未配置A5X4', NULL, NULL, '暂未配置报警信息A5X4', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (137, 3, 'WarningA5X5', '暂未配置A5X5', NULL, NULL, '暂未配置报警信息A5X5', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (138, 3, 'WarningA5X6', '暂未配置A5X6', NULL, NULL, '暂未配置报警信息A5X6', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (139, 3, 'WarningA5X7', '暂未配置A5X7', NULL, NULL, '暂未配置报警信息A5X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (140, 3, 'WarningA6X0', '暂未配置A6X0', NULL, NULL, '暂未配置报警信息A6X0', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (141, 3, 'WarningA6X1', '暂未配置A6X1', NULL, NULL, '暂未配置报警信息A6X1', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (142, 3, 'WarningA6X2', '暂未配置A6X2', NULL, NULL, '暂未配置报警信息A6X2', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (143, 3, 'WarningA6X3', '暂未配置A6X3', NULL, NULL, '暂未配置报警信息A6X3', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (144, 3, 'WarningA6X4', '暂未配置A6X4', NULL, NULL, '暂未配置报警信息A6X4', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (145, 3, 'WarningA6X5', '暂未配置A6X5', NULL, NULL, '暂未配置报警信息A6X5', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (146, 3, 'WarningA6X6', '暂未配置A6X6', NULL, NULL, '暂未配置报警信息A6X6', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (147, 3, 'WarningA6X7', '暂未配置A6X7', NULL, NULL, '暂未配置报警信息A6X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (148, 3, 'WarningA7X0', '暂未配置A7X0', NULL, NULL, '暂未配置报警信息A7X0', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (149, 3, 'WarningA7X1', '暂未配置A7X1', NULL, NULL, '暂未配置报警信息A7X1', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (150, 3, 'WarningA7X2', '暂未配置A7X2', NULL, NULL, '暂未配置报警信息A7X2', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (151, 3, 'WarningA7X3', '暂未配置A7X3', NULL, NULL, '暂未配置报警信息A7X3', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (152, 3, 'WarningA7X4', '暂未配置A7X4', NULL, NULL, '暂未配置报警信息A7X4', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (153, 3, 'WarningA7X5', '暂未配置A7X5', NULL, NULL, '暂未配置报警信息A7X5', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (154, 3, 'WarningA7X6', '暂未配置A7X6', NULL, NULL, '暂未配置报警信息A7X6', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (155, 3, 'WarningA7X7', '暂未配置A7X7', NULL, NULL, '暂未配置报警信息A7X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (156, 3, 'WarningA8X0', '暂未配置A8X0', NULL, NULL, '暂未配置报警信息A8X0', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (157, 3, 'WarningA8X1', '暂未配置A8X1', NULL, NULL, '暂未配置报警信息A8X1', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (158, 3, 'WarningA8X2', '暂未配置A8X2', NULL, NULL, '暂未配置报警信息A8X2', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (159, 3, 'WarningA8X3', '暂未配置A8X3', NULL, NULL, '暂未配置报警信息A8X3', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (160, 3, 'WarningA8X4', '暂未配置A8X4', NULL, NULL, '暂未配置报警信息A8X4', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (161, 3, 'WarningA8X5', '暂未配置A8X5', NULL, NULL, '暂未配置报警信息A8X5', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (162, 3, 'WarningA8X6', '暂未配置A8X6', NULL, NULL, '暂未配置报警信息A8X6', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (163, 3, 'WarningA8X7', '暂未配置A8X7', NULL, NULL, '暂未配置报警信息A8X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (164, 3, 'WarningA9X0', '暂未配置A9X0', NULL, NULL, '暂未配置报警信息A9X0', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (165, 3, 'WarningA9X1', '暂未配置A9X1', NULL, NULL, '暂未配置报警信息A9X1', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (166, 3, 'WarningA9X2', '暂未配置A9X2', NULL, NULL, '暂未配置报警信息A9X2', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (167, 3, 'WarningA9X3', '暂未配置A9X3', NULL, NULL, '暂未配置报警信息A9X3', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (168, 3, 'WarningA9X4', '暂未配置A9X4', NULL, NULL, '暂未配置报警信息A9X4', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (169, 3, 'WarningA9X5', '暂未配置A9X5', NULL, NULL, '暂未配置报警信息A9X5', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (170, 3, 'WarningA9X6', '暂未配置A9X6', NULL, NULL, '暂未配置报警信息A9X6', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (171, 3, 'WarningA9X7', '暂未配置A9X7', NULL, NULL, '暂未配置报警信息A9X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (172, 3, 'WarningA10X0', '暂未配置A10X0', NULL, NULL, '暂未配置报警信息A10X0', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (173, 3, 'WarningA10X1', '暂未配置A10X1', NULL, NULL, '暂未配置报警信息A10X1', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (174, 3, 'WarningA10X2', '暂未配置A10X2', NULL, NULL, '暂未配置报警信息A10X2', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (175, 3, 'WarningA10X3', '暂未配置A10X3', NULL, NULL, '暂未配置报警信息A10X3', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (176, 3, 'WarningA10X4', '暂未配置A10X4', NULL, NULL, '暂未配置报警信息A10X4', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (177, 3, 'WarningA10X5', '暂未配置A10X5', NULL, NULL, '暂未配置报警信息A10X5', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (178, 3, 'WarningA10X6', '暂未配置A10X6', NULL, NULL, '暂未配置报警信息A10X6', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (179, 3, 'WarningA10X7', '暂未配置A10X7', NULL, NULL, '暂未配置报警信息A10X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (180, 3, 'WarningF_A1X0', '码盘故障', NULL, NULL, '码盘故障，请尝试手动复位设备，消除报警', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (181, 3, 'WarningF_A1X1', '急停触发', NULL, NULL, '急停触发，急停开关是否误触发？是否有异常认为打开急停开关？', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (182, 3, 'WarningF_A1X2', '下转侧对位接近开关异常', NULL, NULL, '下转侧对位接近开关异常，检查光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (183, 3, 'WarningF_A1X3', '上砖侧对位接近开关异常', NULL, NULL, '上砖侧对位接近开关异常，检查光电是否误触发', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (184, 3, 'WarningF_A1X4', '前进防撞触发', NULL, NULL, '前进防撞触发，防撞光电亮黄绿灯时，请检查设备前方半米内是否有障碍物', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (185, 3, 'WarningF_A1X5', '后退防撞触发', NULL, NULL, '后退防撞触发，防撞光电亮黄绿灯时，请检查设备后方半米内是否有障碍物', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (186, 3, 'WarningF_A1X6', '暂未配置摆渡A1X6', NULL, NULL, '暂未配置摆渡报警信息A1X6', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (187, 3, 'WarningF_A1X7', '暂未配置摆渡A1X7', NULL, NULL, '暂未配置摆渡报警信息A1X7', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (200, 3, 'DeviceOffline', '设备离线', NULL, NULL, '设备离线', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (201, 3, 'TrackFullButNoneStock', '轨道满砖但没库存', NULL, NULL, '轨道满砖但没库存', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (202, 3, 'CarrierLoadSortTask', '小车倒库中但是小车有货', NULL, NULL, '小车倒库中但是小车有货', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (203, 3, 'CarrierLoadNotSortTask', '小车倒库中任务清除', NULL, NULL, '小车倒库中任务清除', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (204, 3, 'TileNoneStrategy', '砖机没有设置策略', NULL, NULL, '砖机没有设置策略', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (205, 3, 'CarrierFullSignalFullNotOnStoreTrack', '小车满砖信号不在储砖轨道', NULL, NULL, '小车满砖信号不在储砖轨道', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (206, 3, 'CarrierGiveMissTrack', '小车前进放货没有扫到地标', NULL, NULL, '前进放砖没扫到地标,请手动下降放货，移回轨道头', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (207, 3, 'DownTileHaveNotTrackToStore', '砖机找不到空闲轨道存放', NULL, NULL, '砖机找不到合适轨道（品种及状态允许且无任务锁定）存砖', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (208, 3, 'UpTileHaveNotStockToOut', '砖机找不到库存出库', NULL, NULL, '砖机找不到合适库存上砖', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (209, 3, 'TrackEarlyFull', '轨道提前满砖报警', NULL, NULL, '轨道提前满砖警告', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (210, 3, 'UpTileHaveNoTrackToOut', '砖机找不到有砖轨道上砖', NULL, NULL, '砖机找不到合适轨道（品种及状态允许且无任务锁定）上砖', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (211, 3, 'CarrierLoadNeedTakeCare', '小车没任务，有货需要手动处理', NULL, NULL, '小车没任务但有货，需要手动处理', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (212, 3, 'HaveOtherCarrierInSortTrack', '有别的小车在倒库轨道，倒库车已经停止', NULL, NULL, '有别的小车在倒库轨道，倒库小车已经停止', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (213, 3, 'CarrierSortButStop', '倒库小车任务终止，需要手动发送倒库', NULL, NULL, '倒库小车任务终止，需要手动发送倒库', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (214, 3, 'TileMixLastTrackInTrans', '砖机混砖作业，轨道被占用', NULL, NULL, '砖机混砖作业，轨道被占用', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (215, 3, 'TileGoodsIsZero', '砖机工位品种反馈异常', NULL, NULL, '砖机工位品种反馈异常，尝试使用PC当前品种', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (216, 3, 'TileGoodsIsNull', '砖机工位品种没有配置', NULL, NULL, '砖机工位品种没有配置，尝试使用PC当前品种', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (217, 3, 'TransHaveNotTheGiveTrack', '任务进行中没有发现合适的轨道卸砖', NULL, NULL, '任务中没有合适轨道（品种及状态允许且无任务锁定）卸砖', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (218, 3, 'UpTilePreGoodNotSet', '上砖机未选预设品种，未能自动转产', NULL, NULL, '上砖机未选预设品种，未能自动转产', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (219, 3, 'DeviceSortRunOutTrack', '运输车倒库没有扫到定位点冲出轨道', NULL, NULL, '运输车倒库没有扫到定位点冲出轨道', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (220, 3, 'FerryNoLocation', '摆渡车失去位置信息', NULL, NULL, '摆渡车失去位置信息，为安全起见已停止所有任务及指令的执行，待恢复位置信息后再继续作业，请检查设备！', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (221, 3, 'FailAllocateCarrier', '运输车分配失败', NULL, NULL, '运输车分配失败', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (222, 3, 'FailAllocateFerry', '分配摆渡车失败', NULL, NULL, '分配摆渡车失败', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (223, 3, 'UpTileEmptyNeedAndNoBack', '任务中上砖机没了需求且小车无轨可回', NULL, NULL, '任务中上砖机没了需求且小车无轨可回', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (224, 3, 'FerryTargetUnconfigured', '摆渡车目的位置没有对位坐标值', NULL, NULL, '摆渡车目的位置没有对位坐标值，请操作重新对一次轨道位置', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (225, 3, 'CarrierFreeButMoveInFerry', '运输车空闲状态(停止/指令完成)但处于上下摆渡中', NULL, NULL, '运输车空闲状态(停止/指令完成)但处于上下摆渡中，无法解锁相应摆渡车', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (226, 3, 'CarrierFreeInFerryButLocErr', '运输车空闲状态(停止/指令完成)在摆渡上，但当前轨道有误', NULL, NULL, '运输车空闲状态(停止/指令完成)在摆渡上，但当前轨道有误，无法解锁相应摆渡车', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (227, 3, 'DownTaskSwitchClosed', '【下砖任务开关】关闭', NULL, NULL, '【下砖任务开关】已关闭', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (228, 3, 'UpTaskSwitchClosed', '【上砖任务开关】关闭', NULL, NULL, '【上砖任务开关】已关闭', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (229, 3, 'SortTaskSwitchClosed', '【倒库任务开关】关闭', NULL, NULL, '【倒库任务开关】已关闭', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (230, 3, 'TheEarliestStockInDown', '最早的库存在下砖入库侧轨道', NULL, NULL, '以先进先出为原则，发现最早的库存在下砖入库侧轨道，暂无法上砖', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (231, 3, 'PreventTimeConflict', '不能连续下砖但仅剩最后一条轨道', NULL, NULL, '不允许同品种下砖连续下满同一条轨道，需变更轨道下砖，防止时间冲突', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (232, 3, 'SortFinishButDownExistStock', '倒库指令已完成，入库还有库存', NULL, NULL, '运输车倒库完成后入库轨道还有库存，请在核实并修改入库轨道的库存之后，1.如果需要继续倒库，请手动给运输车发倒库任务，2.如果不需要继续倒库，请取消当前轨道的倒库任务和修改轨道状态为有砖/空砖', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (233, 3, 'GetStockButNull', '取砖指令完成后，没有取到砖', NULL, NULL, '运输车取砖空砖，请检查取砖光电是否异常不亮，(如运输车在储砖轨道请核实轨道库存)，最后给运输车发终止指令', NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for ferry_pos
-- ----------------------------
DROP TABLE IF EXISTS `ferry_pos`;
CREATE TABLE `ferry_pos`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '轨道ID',
  `device_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '设备ID',
  `ferry_code` smallint(4) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡编码',
  `ferry_pos` int(11) NULL DEFAULT NULL COMMENT '实际坐标',
  `old_ferry_pos` int(11) NULL DEFAULT NULL COMMENT '旧的设置坐标',
  `sim_ferry_pos` int(255) NULL DEFAULT NULL COMMENT '摆渡车模拟脉冲值',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fepos_traid_idx`(`track_id`) USING BTREE,
  INDEX `fepos_devid_idx`(`device_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 16 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '摆渡车对位表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ferry_pos
-- ----------------------------
INSERT INTO `ferry_pos` VALUES (1, 1, 3, 514, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (2, 2, 3, 513, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (3, 3, 3, 512, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (4, 4, 3, 511, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (5, 5, 3, 501, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (6, 6, 3, 502, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (7, 7, 3, 503, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (8, 8, 3, 504, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (9, 9, 3, 505, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (10, 10, 3, 506, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (11, 11, 3, 507, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (12, 12, 3, 508, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (13, 13, 3, 509, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (14, 14, 3, 510, 0, NULL, NULL);
INSERT INTO `ferry_pos` VALUES (15, 16, 3, 301, 0, NULL, NULL);

-- ----------------------------
-- Table structure for good_size
-- ----------------------------
DROP TABLE IF EXISTS `good_size`;
CREATE TABLE `good_size`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '规格ID',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '规格名称',
  `length` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '长',
  `width` smallint(5) NULL DEFAULT NULL COMMENT '宽',
  `stack` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '垛',
  `car_lenght` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '一车砖的长度（脉冲）',
  `car_space` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '砖与砖安全间距（脉冲）',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '品种规格表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of good_size
-- ----------------------------
INSERT INTO `good_size` VALUES (1, '750x1500', 1500, 750, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (2, '900x1800', 1800, 900, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (3, '900x900', 900, 900, 4, NULL, NULL);

-- ----------------------------
-- Table structure for goods
-- ----------------------------
DROP TABLE IF EXISTS `goods`;
CREATE TABLE `goods`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `area_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '区域',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '品种名称',
  `color` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '色号',
  `pieces` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '满砖数',
  `carriertype` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '运输车类型',
  `memo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `updatetime` datetime(0) NULL DEFAULT NULL,
  `minstack` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '最少托数',
  `size_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '规格ID',
  `level` tinyint(3) UNSIGNED NOT NULL COMMENT '等级',
  `createtime` datetime(0) NULL DEFAULT NULL COMMENT '创建时间',
  `top` bit(1) NULL DEFAULT NULL COMMENT '是否置顶',
  `empty` bit(1) NULL DEFAULT NULL COMMENT '是否是空品种',
  `info` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '品种长信息',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `g_createtime_idx`(`createtime`) USING BTREE,
  INDEX `g_top_idx`(`top`) USING BTREE,
  INDEX `fk_size_id`(`size_id`) USING BTREE,
  CONSTRAINT `fk_size_id` FOREIGN KEY (`size_id`) REFERENCES `good_size` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '品种表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of goods
-- ----------------------------

-- ----------------------------
-- Table structure for line
-- ----------------------------
DROP TABLE IF EXISTS `line`;
CREATE TABLE `line`  (
  `id` smallint(5) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '序列号',
  `area_id` int(11) UNSIGNED NOT NULL COMMENT '区域ID',
  `line` smallint(5) UNSIGNED NOT NULL COMMENT '线',
  `name` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '名称',
  `sort_task_qty` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '倒库任务数量',
  `up_task_qty` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '上砖任务数量',
  `down_task_qty` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '下砖任务数量',
  `max_upsort_num` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '上砖接力最大倒库数量',
  `onoff_up` bit(1) NULL DEFAULT NULL COMMENT '上砖开关',
  `onoff_down` bit(1) NULL DEFAULT NULL COMMENT '下砖开关',
  `onoff_sort` bit(1) NULL DEFAULT NULL COMMENT '倒库开关',
  `line_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '线类型：0窑后 1包装前',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '产线表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of line
-- ----------------------------
INSERT INTO `line` VALUES (1, 1, 1, '包装线', 0, 1, 2, NULL, b'0', b'0', b'0', 1);

-- ----------------------------
-- Table structure for rf_client
-- ----------------------------
DROP TABLE IF EXISTS `rf_client`;
CREATE TABLE `rf_client`  (
  `rfid` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户端唯一标识',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `ip` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'IP',
  `conn_time` datetime(0) NULL DEFAULT NULL COMMENT '最近连接时间',
  `disconn_time` datetime(0) NULL DEFAULT NULL COMMENT '最近离线时间',
  `filter_area` bit(1) NULL DEFAULT NULL COMMENT '是否过滤区域',
  `filter_areaids` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '过滤的区域ID们',
  `filter_type` bit(1) NULL DEFAULT NULL COMMENT '是否过滤设备类型',
  `filter_typevalues` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '过滤的设备类型们',
  `filter_dev` bit(1) NULL DEFAULT NULL COMMENT '是否过滤指定设备',
  `filter_devids` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '过滤指定设备的ID',
  PRIMARY KEY (`rfid`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '移动客户端表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of rf_client
-- ----------------------------

-- ----------------------------
-- Table structure for stock
-- ----------------------------
DROP TABLE IF EXISTS `stock`;
CREATE TABLE `stock`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `goods_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '品种ID',
  `stack` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '垛数',
  `pieces` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '片数',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '储砖轨道ID',
  `produce_time` datetime(0) NULL DEFAULT NULL COMMENT '生产时间',
  `pos` smallint(6) NULL DEFAULT NULL COMMENT '同轨道中的位置 1-100',
  `pos_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '砖在轨道的位置：顶部/中间/底部',
  `tilelifter_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '下砖机ID',
  `area` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域',
  `track_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '轨道类型',
  `location` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '库存实际坐标（脉冲）',
  `location_cal` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '库存计算坐标（脉冲）',
  `last_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '储砖轨道ID',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `sto_goods_id_fk`(`goods_id`) USING BTREE,
  INDEX `sto_track_id_fk`(`track_id`) USING BTREE,
  CONSTRAINT `sto_goods_id_fk` FOREIGN KEY (`goods_id`) REFERENCES `goods` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `sto_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 43 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '库存表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock
-- ----------------------------

-- ----------------------------
-- Table structure for stock_log
-- ----------------------------
DROP TABLE IF EXISTS `stock_log`;
CREATE TABLE `stock_log`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `goods_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '品种ID',
  `stack` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '垛数',
  `pieces` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '片数',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '储砖轨道ID',
  `tilelifter_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '下砖机ID',
  `create_time` datetime(0) NULL DEFAULT NULL COMMENT '生产时间/消耗时间',
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
  `use` bit(1) NULL DEFAULT NULL COMMENT '数据处理标志',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '库存记录表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_log
-- ----------------------------

-- ----------------------------
-- Table structure for stock_trans
-- ----------------------------
DROP TABLE IF EXISTS `stock_trans`;
CREATE TABLE `stock_trans`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '交易ID',
  `trans_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '交易类型 1下砖交易 2 上砖交易',
  `trans_status` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '交易状态',
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
  `line` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '线：区域 > 线',
  `goods_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '品种ID',
  `stock_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '库存ID',
  `take_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '取砖轨道ID',
  `give_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '放砖轨道ID',
  `tilelifter_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上/下砖机ID',
  `take_ferry_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '取货摆渡车ID',
  `give_ferry_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '放货摆渡车ID',
  `carrier_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '运输车ID',
  `create_time` datetime(0) NULL DEFAULT NULL COMMENT '生产时间',
  `load_time` datetime(0) NULL DEFAULT NULL COMMENT '取货时间',
  `unload_time` datetime(0) NULL DEFAULT NULL COMMENT '卸货时间',
  `finish` bit(1) NULL DEFAULT NULL COMMENT '是否完成',
  `finish_time` datetime(0) NULL DEFAULT NULL COMMENT '完成时间',
  `cancel` bit(1) NULL DEFAULT NULL COMMENT '是否取消',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `tran_produce_time_idx`(`create_time`) USING BTREE,
  INDEX `tran_type_idx`(`trans_type`) USING BTREE,
  INDEX `tran_status_idx`(`trans_status`) USING BTREE,
  INDEX `tran_finish_idx`(`finish`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '任务表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_trans
-- ----------------------------

-- ----------------------------
-- Table structure for tile_track
-- ----------------------------
DROP TABLE IF EXISTS `tile_track`;
CREATE TABLE `tile_track`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '砖机轨道',
  `tile_id` int(11) UNSIGNED NOT NULL COMMENT '砖机ID',
  `track_id` int(11) UNSIGNED NOT NULL,
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '优先级',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '砖机-轨道-作业表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tile_track
-- ----------------------------

-- ----------------------------
-- Table structure for tilelifterneed
-- ----------------------------
DROP TABLE IF EXISTS `tilelifterneed`;
CREATE TABLE `tilelifterneed`  (
  `device_id` int(10) UNSIGNED NOT NULL COMMENT '上/下砖机ID',
  `track_id` int(10) NOT NULL COMMENT '需求轨道',
  `left` bit(1) NULL DEFAULT NULL COMMENT '是否左需求',
  `trans_id` int(10) NULL DEFAULT NULL COMMENT '生成的任务id',
  `create_time` datetime(0) NULL DEFAULT NULL COMMENT '需求生成时间',
  `trans_create_time` datetime(0) NULL DEFAULT NULL COMMENT '任务生成时间',
  `finish` bit(1) NULL DEFAULT NULL COMMENT '是否完成',
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '砖机类型',
  `area_id` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域值用于过滤'
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '砖机需求表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tilelifterneed
-- ----------------------------

-- ----------------------------
-- Table structure for track
-- ----------------------------
DROP TABLE IF EXISTS `track`;
CREATE TABLE `track`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `area` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域：过滤作用',
  `line` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '线：区域 > 线',
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '类型',
  `stock_status` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '状态',
  `track_status` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '轨道状态停用/启用/倒库中',
  `width` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道宽度',
  `left_distance` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '距离左轨道间距',
  `right_distance` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '距离左轨道间距',
  `ferry_up_code` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车上砖测轨道编码',
  `ferry_down_code` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车下砖测轨道编码',
  `max_store` int(6) UNSIGNED NULL DEFAULT NULL COMMENT '最大存储数量',
  `brother_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '相邻轨道ID',
  `left_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '靠左轨道ID',
  `right_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '靠右轨道ID',
  `memo` smallint(6) NULL DEFAULT NULL COMMENT '备注',
  `rfid_1` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `rfid_2` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `rfid_3` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `rfid_4` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `rfid_5` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `rfid_6` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `rfids` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '轨道所有地标（用 # 隔开）',
  `split_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道分段坐标点',
  `limit_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道下砖极限坐标点',
  `limit_point_up` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道上砖极限坐标点',
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '顺序',
  `recent_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '最近上砖/下砖规格',
  `recent_tileid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '最近上/下砖机ID',
  `alert_status` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '故障状态',
  `alert_carrier` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '故障小车',
  `alert_trans` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '故障任务',
  `early_full` bit(1) NULL DEFAULT NULL COMMENT '提前满砖',
  `full_time` datetime(0) NULL DEFAULT NULL COMMENT '满砖时间',
  `same_side_inout` bit(1) NULL DEFAULT NULL COMMENT '是否同侧出入库',
  `upcount` int(11) NULL DEFAULT NULL COMMENT '上砖车数计数',
  `up_split_point` smallint(6) UNSIGNED NULL DEFAULT NULL COMMENT '上砖分段坐标点',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 17 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '轨道表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of track
-- ----------------------------
INSERT INTO `track` VALUES (1, '514上下砖轨', 1, 1, 1, 0, 1, 0, 0, 0, 514, 514, 1, 0, 0, 0, NULL, 514, 514, NULL, NULL, NULL, NULL, '514', NULL, 1206, 1828, 15, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (2, '513上下砖轨', 1, 1, 1, 0, 1, 0, 0, 0, 513, 513, 1, 0, 0, 0, NULL, 513, 513, NULL, NULL, NULL, NULL, '513', NULL, 1206, 1828, 13, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (3, '512下砖轨', 1, 1, 1, 0, 1, 0, 0, 0, 512, 512, 1, 0, 0, 0, NULL, 512, 512, NULL, NULL, NULL, NULL, '512', NULL, 1206, 1828, 12, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (4, '511下砖轨', 1, 1, 1, 0, 1, 0, 0, 0, 511, 511, 1, 0, 0, 0, NULL, 511, 511, NULL, NULL, NULL, NULL, '511', NULL, 1206, 1828, 11, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (5, '501储砖轨', 1, 1, 4, 2, 1, 700, 260, 260, 501, 510, 100, 0, 0, 6, NULL, 510, 510, NULL, NULL, NULL, NULL, '501', 2023, 1301, 2751, 10, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (6, '502储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 502, 509, 100, 0, 5, 7, NULL, 509, 509, NULL, NULL, NULL, NULL, '502', 2023, 1301, 2751, 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (7, '503储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 503, 508, 100, 0, 6, 8, NULL, 508, 508, NULL, NULL, NULL, NULL, '503', 2023, 1301, 2751, 8, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (8, '504储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 504, 507, 100, 0, 7, 9, NULL, 507, 507, NULL, NULL, NULL, NULL, '504', 2023, 1301, 2751, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (9, '505储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 505, 506, 100, 0, 8, 10, NULL, 506, 506, NULL, NULL, NULL, NULL, '505', 2023, 1301, 2751, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (10, '506储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 506, 505, 100, 0, 9, 11, NULL, 505, 505, NULL, NULL, NULL, NULL, '506', 2023, 1301, 2751, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (11, '507储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 507, 504, 100, 0, 10, 12, NULL, 504, 504, NULL, NULL, NULL, NULL, '507', 2023, 1301, 2751, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (12, '508储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 508, 503, 100, 0, 11, 13, NULL, 503, 503, NULL, NULL, NULL, NULL, '508', 2023, 1301, 2751, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (13, '509储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 509, 502, 100, 0, 12, 14, NULL, 502, 502, NULL, NULL, NULL, NULL, '509', 2023, 1301, 2751, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (14, '510储砖轨', 1, 1, 4, 0, 1, 700, 260, 260, 510, 501, 100, 0, 13, 0, NULL, 501, 501, NULL, NULL, NULL, NULL, '510', 2023, 1301, 2751, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b'1', NULL, NULL);
INSERT INTO `track` VALUES (15, '1B51摆轨', 1, 1, 5, 0, 1, 0, 0, 0, 401, 401, 1, 0, 0, 0, NULL, 401, 401, NULL, NULL, NULL, NULL, '401', NULL, 1000, 1000, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (16, '维修轨', 1, 1, 1, 0, 0, 0, 0, 0, 301, 301, 1, 0, 0, 0, NULL, 301, 301, NULL, NULL, NULL, NULL, '301', NULL, 1307, 1307, 14, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for track_log
-- ----------------------------
DROP TABLE IF EXISTS `track_log`;
CREATE TABLE `track_log`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '轨道ID',
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '日志类型 1空砖 2满砖',
  `dev_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '设备ID',
  `stock_count` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '库存数量',
  `log_time` datetime(0) NULL DEFAULT NULL COMMENT '日志时间',
  `memo` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `area` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '区域',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '轨道记录表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of track_log
-- ----------------------------

-- ----------------------------
-- Table structure for traffic_control
-- ----------------------------
DROP TABLE IF EXISTS `traffic_control`;
CREATE TABLE `traffic_control`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `area` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域',
  `traffic_control_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '交管类型',
  `restricted_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '被交管 设备ID',
  `control_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '交管 设备ID',
  `traffic_control_status` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '交管状态（0：交管中，1：已完成）',
  `from_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '交管起始轨道ID',
  `from_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '交管起始点位',
  `from_site` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '交管起始坐标',
  `to_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '交管结束轨道ID',
  `to_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '交管结束点位',
  `to_site` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '交管结束坐标',
  `create_time` datetime(0) NULL DEFAULT NULL COMMENT '创建时间',
  `update_time` datetime(0) NULL DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fk_create_time_index`(`create_time`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '交管表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of traffic_control
-- ----------------------------

-- ----------------------------
-- Table structure for warning
-- ----------------------------
DROP TABLE IF EXISTS `warning`;
CREATE TABLE `warning`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `area_id` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
  `line_id` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '线路ID',
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '报警类型',
  `resolve` bit(1) NULL DEFAULT NULL COMMENT '是否解决',
  `dev_id` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '设备',
  `track_id` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道ID',
  `trans_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '任务',
  `content` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `createtime` datetime(0) NULL DEFAULT NULL COMMENT '报警时间',
  `resolvetime` datetime(0) NULL DEFAULT NULL COMMENT '解决时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `w_createtime_idx`(`createtime`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '警告表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of warning
-- ----------------------------

-- ----------------------------
-- Table structure for wcs_menu
-- ----------------------------
DROP TABLE IF EXISTS `wcs_menu`;
CREATE TABLE `wcs_menu`  (
  `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `prior` smallint(3) NULL DEFAULT NULL COMMENT '优先级',
  `memo` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '菜单表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_menu
-- ----------------------------
INSERT INTO `wcs_menu` VALUES (1, '基础菜单', 1, '无特殊权限，通用菜单');
INSERT INTO `wcs_menu` VALUES (2, '管理员菜单', 2, '现场管理员');
INSERT INTO `wcs_menu` VALUES (3, '超级管理员菜单', 3, '超级管理员菜单');

-- ----------------------------
-- Table structure for wcs_menu_dtl
-- ----------------------------
DROP TABLE IF EXISTS `wcs_menu_dtl`;
CREATE TABLE `wcs_menu_dtl`  (
  `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT,
  `menu_id` int(6) NULL DEFAULT NULL,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `folder` bit(1) NULL DEFAULT NULL COMMENT '是否是文件夹',
  `folder_id` int(6) NULL DEFAULT NULL COMMENT '所属文件夹ID',
  `module_id` int(6) NULL DEFAULT NULL COMMENT '模块KEY',
  `order` smallint(3) NULL DEFAULT NULL,
  `rf` bit(1) NULL DEFAULT NULL COMMENT '是否是平板的',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 106 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '菜单明细表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_menu_dtl
-- ----------------------------
INSERT INTO `wcs_menu_dtl` VALUES (1, 1, '任务', b'1', 0, 0, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (2, 0, '任务开关', b'0', 1, 3, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (3, 0, '任务状态', b'0', 1, 11, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (4, 1, '设备', b'1', 0, 0, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (5, 0, '砖机', b'0', 4, 6, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (6, 0, '摆渡车', b'0', 4, 4, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (7, 0, '运输车', b'0', 4, 5, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (8, 1, '轨道', b'1', 0, 7, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (9, 0, '轨道状态', b'0', 8, 7, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (10, 0, '库存修改', b'0', 8, 10, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (11, 1, '统计', b'1', 0, 0, 4, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (12, 0, '轨道库存', b'0', 11, 15, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (13, 0, '警告信息', b'0', 11, 16, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (14, 0, '空满轨道', b'0', 11, 17, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (15, 1, '配置', b'1', 0, 0, 5, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (16, 0, '摆渡对位', b'0', 15, 13, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (17, 2, '任务', b'1', 0, 0, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (18, 0, '任务开关', b'0', 17, 3, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (19, 0, '任务状态', b'0', 17, 11, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (20, 2, '设备', b'1', 0, 0, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (21, 0, '砖机', b'0', 20, 6, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (22, 0, '摆渡车', b'0', 20, 4, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (23, 0, '运输车', b'0', 20, 5, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (24, 2, '轨道', b'1', 0, 0, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (25, 0, '轨道状态', b'0', 24, 7, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (26, 0, '库存修改', b'0', 24, 10, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (27, 2, '统计', b'1', 0, 0, 4, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (28, 0, '轨道库存', b'0', 27, 15, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (29, 0, '警告信息', b'0', 27, 16, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (30, 0, '空满轨道', b'0', 27, 17, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (31, 2, '配置', b'1', 0, 0, 5, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (32, 0, '摆渡对位', b'0', 31, 13, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (33, 0, '区域配置', b'0', 31, 2, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (35, 0, '字典', b'0', 31, 29, 4, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (36, 0, '测可入砖', b'0', 31, 9, 5, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (38, 2, '授权', b'1', 0, 0, 6, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (39, 0, '用户', b'0', 38, 31, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (40, 0, '菜单', b'0', 38, 30, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (41, 3, '任务', b'1', 0, 0, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (42, 0, '任务开关', b'0', 41, 3, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (43, 0, '任务状态', b'0', 41, 11, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (44, 3, '设备', b'1', 0, 0, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (45, 0, '砖机', b'0', 44, 6, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (46, 0, '摆渡车', b'0', 44, 4, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (47, 0, '运输车', b'0', 44, 5, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (48, 3, '轨道', b'1', 0, 0, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (49, 0, '轨道状态', b'0', 48, 7, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (50, 0, '库存修改', b'0', 48, 10, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (51, 3, '统计', b'1', 0, 0, 4, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (52, 0, '轨道库存', b'0', 51, 15, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (53, 0, '警告信息', b'0', 51, 16, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (54, 0, '空满轨道', b'0', 51, 17, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (55, 3, '配置', b'1', 0, 0, 5, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (56, 0, '摆渡对位', b'0', 55, 13, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (57, 0, '区域配置', b'0', 55, 2, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (58, 0, '轨道分配', b'0', 55, 14, 4, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (59, 0, '字典', b'0', 55, 29, 5, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (60, 0, '测可入砖', b'0', 55, 9, 7, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (61, 0, '添加任务', b'0', 55, 12, 8, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (62, 3, '授权', b'1', 0, 0, 6, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (63, 0, '用户', b'0', 62, 31, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (64, 0, '菜单', b'0', 62, 30, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (89, 0, '品种管理', b'0', 15, 8, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (90, 1, '品种设置', b'0', 0, 19, 8, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (91, 1, '轨道设置', b'0', 0, 20, 14, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (92, 1, '砖机规格', b'0', 0, 21, 9, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (93, 1, '任务开关', b'0', 0, 22, 6, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (94, 1, '摆渡车状态', b'0', 0, 23, 11, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (95, 1, '运输车状态', b'0', 0, 24, 12, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (96, 1, '砖机状态', b'0', 0, 25, 13, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (97, 1, '轨道库存', b'0', 0, 26, 15, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (98, 1, '任务信息', b'0', 0, 27, 7, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (99, 1, '摆渡车对位', b'0', 0, 32, 10, b'1');
INSERT INTO `wcs_menu_dtl` VALUES (100, 0, '轨道分配', b'0', 24, 14, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (102, 0, '复位地标', b'0', 31, 33, 6, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (103, 0, '品种管理', b'0', 31, 8, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (104, 0, '复位地标', b'0', 55, 33, 6, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (105, 0, '品种管理', b'0', 55, 8, 1, b'0');

-- ----------------------------
-- Table structure for wcs_module
-- ----------------------------
DROP TABLE IF EXISTS `wcs_module`;
CREATE TABLE `wcs_module`  (
  `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '模块名称',
  `type` tinyint(3) NULL DEFAULT NULL COMMENT '类型：PC, RF, 电视..',
  `key` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '模块对应界面key',
  `entity` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '平板-模块类名',
  `brush` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `geometry` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT 'Tab图标',
  `winctlname` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT 'PC调度界面文件名',
  `memo` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 35 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '模块表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_module
-- ----------------------------
INSERT INTO `wcs_module` VALUES (1, '主页', 0, 'Home', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'HomeCtl', '启动调度自动打开');
INSERT INTO `wcs_module` VALUES (2, '区域', 0, 'Area', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'AreaCtl', '配置砖机/摆渡车轨道');
INSERT INTO `wcs_module` VALUES (3, '开关', 0, 'AreaSwitch', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'AreaSwitchCtl', '任务开关控制');
INSERT INTO `wcs_module` VALUES (4, '摆渡车', 0, 'Ferry', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'FerryCtl', '摆渡车详细状态信息');
INSERT INTO `wcs_module` VALUES (5, '运输车', 0, 'Carrier', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'CarrierCtl', '运输车详细状态信息');
INSERT INTO `wcs_module` VALUES (6, '上下砖机', 0, 'TileLifter', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TileLifterCtl', '上下砖机详细状态信息');
INSERT INTO `wcs_module` VALUES (7, '轨道信息', 0, 'Track', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TrackCtl', '轨道状态信息');
INSERT INTO `wcs_module` VALUES (8, '品种', 0, 'Goods', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'GoodsCtl', '品种详细信息');
INSERT INTO `wcs_module` VALUES (9, '测可入砖', 0, 'TestGood', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TestGoodCtl', '测试轨道是否可以放指定的砖');
INSERT INTO `wcs_module` VALUES (10, '轨道库存', 0, 'Stock', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'StockCtl', '单一轨道库存的详细信息');
INSERT INTO `wcs_module` VALUES (11, '任务', 0, 'Trans', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TransCtl', '调度设备的任务信息,状态');
INSERT INTO `wcs_module` VALUES (12, '添加任务', 0, 'AddManualTrans', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'AddManualTransCtl', '手动添加任务');
INSERT INTO `wcs_module` VALUES (13, '摆渡对位', 0, 'FerryPos', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'FerryPosCtl', '摆渡车对位，复位');
INSERT INTO `wcs_module` VALUES (14, '轨道分配', 0, 'TrackAllocate', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TrackAllocateCtl', '查看砖机选定规格的轨道分配');
INSERT INTO `wcs_module` VALUES (15, '库存', 0, 'StockSum', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'StockSumCtl', '所有轨道的库存列表信息');
INSERT INTO `wcs_module` VALUES (16, '警告', 0, 'WarnLog', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'WarnLogCtl', '警告详细信息记录');
INSERT INTO `wcs_module` VALUES (17, '空满轨道', 0, 'TrackLog', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TrackLogCtl', '轨道空满轨道信息记录');
INSERT INTO `wcs_module` VALUES (18, '按轨出库', 0, 'TileTrack', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'TileTrackCtl', '上砖机按轨道上砖使用');
INSERT INTO `wcs_module` VALUES (19, '品种设置', 1, 'RFGOODTYPESETTING', 'com.keda.wcsfixplatformapp.screen.rfgood.RfGoodMainScreen', '', 'goodstype.png', '', '平板-品种查看/添加');
INSERT INTO `wcs_module` VALUES (20, '轨道设置', 1, 'RFTRACK', 'com.keda.wcsfixplatformapp.screen.rftrack.RfTrackScreen', '', 'assignment.png', '', '平板-轨道查看/修改状态');
INSERT INTO `wcs_module` VALUES (21, '砖机品种', 1, 'RFTILEGOOD', 'com.keda.wcsfixplatformapp.screen.rftilegood.RfTileGoodScreen', '', 'updowndev.png', '', '平板-砖机品种查看/修改');
INSERT INTO `wcs_module` VALUES (22, '任务开关', 1, 'RFTASKSWITCH', 'com.keda.wcsfixplatformapp.screen.rfswitch.RfSwitchScreen', '', 'othersetting.png', '', '平板-区域任务开关控制');
INSERT INTO `wcs_module` VALUES (23, '摆渡车状态', 1, 'RFDEVFERRY', 'com.keda.wcsfixplatformapp.screen.rfferry.RfFerryScreen', '', 'othersetting.png', '', '平板-摆渡车详细状态');
INSERT INTO `wcs_module` VALUES (24, '运输车状态', 1, 'RFDEVCARRIER', 'com.keda.wcsfixplatformapp.screen.rfdevcarrier.RfDevCarrierScreen', '', 'shiftcar.png', '', '平板-运输车详细状态');
INSERT INTO `wcs_module` VALUES (25, '砖机状态', 1, 'RFDEVTILELIFTER', 'com.keda.wcsfixplatformapp.screen.rftilelifter.RfTileLifterScreen', '', 'assignment.png', '', '平板-砖机详细状态');
INSERT INTO `wcs_module` VALUES (26, '轨道库存', 1, 'RFTRACKSTOCK', 'com.keda.wcsfixplatformapp.screen.rftrackstock.RfTrackStockScreen', '', 'assignment.png', '', '平板-单一轨道库存查看/添加/删除');
INSERT INTO `wcs_module` VALUES (27, '任务信息', 1, 'RFSTOCKTRANS', 'com.keda.wcsfixplatformapp.screen.rftrans.RfTransScreen', '', 'updowndev.png', '', '平板-任务查看/操作');
INSERT INTO `wcs_module` VALUES (28, '按轨上砖', 1, 'RFTILETRACK', 'com.keda.wcsfixplatformapp.screen.rftiletrack.RfTileTrackScreen', '', 'updowndev.png', '', '平板-上砖机按轨道上砖');
INSERT INTO `wcs_module` VALUES (29, '字典', 0, 'Diction', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'DictionCtl', '字典值查看');
INSERT INTO `wcs_module` VALUES (30, '菜单', 0, 'Menu', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'MenuCtl', 'PC调度菜单配置修改');
INSERT INTO `wcs_module` VALUES (31, '用户', 0, 'User', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'UserCtl', 'PC调度用户配置');
INSERT INTO `wcs_module` VALUES (32, '摆渡车对位', 1, 'RFARFTRACK', 'com.keda.wcsfixplatformapp.screen.rfferrypose.RfFerryPosScreen', '', 'arttoposition.png', NULL, '平板-摆渡车对位');
INSERT INTO `wcs_module` VALUES (33, '复位地标', 0, 'CarrierPos', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'CarrierPosCtl', 'PC运输车复位地标');
INSERT INTO `wcs_module` VALUES (34, '轨道脉冲配置', 0, 'TrackSetPoint', NULL, 'DarkPrimaryBrush', 'ConfigGeometry', 'TrackSetPointCtl', 'PC轨道脉冲配置');

-- ----------------------------
-- Table structure for wcs_role
-- ----------------------------
DROP TABLE IF EXISTS `wcs_role`;
CREATE TABLE `wcs_role`  (
  `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '角色名称',
  `admin` bit(1) NULL DEFAULT NULL COMMENT '是否是管理员角色',
  `menu_id` int(6) NULL DEFAULT NULL COMMENT '菜单',
  `prior` tinyint(3) NULL DEFAULT NULL COMMENT '优先级',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '角色表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_role
-- ----------------------------
INSERT INTO `wcs_role` VALUES (1, '普通角色', b'0', 1, 1);
INSERT INTO `wcs_role` VALUES (2, '管理人员', b'0', 2, 50);
INSERT INTO `wcs_role` VALUES (3, '超级管理员', b'1', 3, 100);

-- ----------------------------
-- Table structure for wcs_user
-- ----------------------------
DROP TABLE IF EXISTS `wcs_user`;
CREATE TABLE `wcs_user`  (
  `id` int(6) UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '用户名',
  `password` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `memo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `role_id` int(6) NULL DEFAULT NULL COMMENT '角色ID',
  `exitwcs` bit(1) NULL DEFAULT NULL COMMENT '能否退出调度',
  `guest` bit(1) NULL DEFAULT NULL COMMENT '默认登陆用户',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '用户表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_user
-- ----------------------------
INSERT INTO `wcs_user` VALUES (1, '123', '123', '操作员', '一般操作', 1, b'1', b'1');
INSERT INTO `wcs_user` VALUES (2, 'admin', 'admin', '管理员', '', 2, b'1', b'0');
INSERT INTO `wcs_user` VALUES (3, 'supervisor', 'supervisor', '超级管理员', '', 3, b'1', b'0');

-- ----------------------------
-- View structure for active_dev
-- ----------------------------
DROP VIEW IF EXISTS `active_dev`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `active_dev` AS select `t`.`id` AS `id`,`t`.`name` AS `name`,`t`.`ip` AS `ip`,`t`.`port` AS `port`,`t`.`type` AS `type`,`t`.`type2` AS `type2`,`t`.`enable` AS `enable`,`t`.`att1` AS `att1`,`t`.`att2` AS `att2`,`t`.`memo` AS `memo` from `device` `t` where (`t`.`enable` = 1);

-- ----------------------------
-- View structure for area_stock_sum
-- ----------------------------
DROP VIEW IF EXISTS `area_stock_sum`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `area_stock_sum` AS select `t`.`goods_id` AS `goods_id`,min(`t`.`produce_time`) AS `produce_time`,count(`t`.`id`) AS `count`,sum(`t`.`pieces`) AS `pieces`,sum(`t`.`stack`) AS `stack`,`t`.`area` AS `area` from `stock` `t` where (`t`.`track_type` in (2,3,4)) group by `t`.`goods_id` order by `t`.`area`,`t`.`goods_id`,`produce_time`,`t`.`track_id`;

-- ----------------------------
-- View structure for stock_sum
-- ----------------------------
DROP VIEW IF EXISTS `stock_sum`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `stock_sum` AS select `t`.`goods_id` AS `goods_id`,`t`.`track_id` AS `track_id`,min(`t`.`produce_time`) AS `produce_time`,count(`t`.`id`) AS `count`,sum(`t`.`pieces`) AS `pieces`,sum(`t`.`stack`) AS `stack`,`t`.`area` AS `area`,`t`.`track_type` AS `track_type` from `stock` `t` where (`t`.`track_type` in (2,3,4)) group by `t`.`track_id`,`t`.`goods_id` order by `t`.`area`,`t`.`goods_id`,`produce_time`,`t`.`track_id`;

-- ----------------------------
-- View structure for tile_gchange_v
-- ----------------------------
DROP VIEW IF EXISTS `tile_gchange_v`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `tile_gchange_v` AS select `c`.`id` AS `id`,`d`.`area` AS `area`,`d`.`name` AS `name`,`d`.`type` AS `type`,`d`.`type2` AS `type2`,`c`.`old_goodid` AS `old_goodid`,`c`.`pre_goodid` AS `pre_goodid`,`c`.`goods_id` AS `goods_id`,`c`.`work_type` AS `work_type`,`c`.`level` AS `level`,`c`.`work_mode` AS `work_mode` from (`config_tilelifter` `c` join `device` `d` on((`c`.`id` = `d`.`id`))) where ((`c`.`id` = `d`.`id`) and (`c`.`good_change` = 1));

-- ----------------------------
-- Procedure structure for DELETE_DATA
-- ----------------------------
DROP PROCEDURE IF EXISTS `DELETE_DATA`;
delimiter ;;
CREATE PROCEDURE `DELETE_DATA`()
BEGIN
	/*仅保留 31 天的报警数据*/
	delete from warning where resolve = 1 and DATEDIFF(CURRENT_DATE,createtime) >= 31;
	/*仅保留 31 天的任务数据*/
	delete from stock_trans where (finish = 1 or cancel = 1) and DATEDIFF(CURRENT_DATE,create_time) >= 31;
	/*仅保留 31 天的记录数据*/
	delete from stock_log where DATEDIFF(CURRENT_DATE,create_time) >= 31;
	delete from track_log where DATEDIFF(CURRENT_DATE,log_time) >= 31;
	delete from consume_log where DATEDIFF(CURRENT_DATE,consume_time) >= 31;
	/*仅保留 7 天的交管数据*/
	delete from traffic_control where DATEDIFF(CURRENT_DATE,create_time) >= 7;
	/*仅保留 7 天的砖机需求数据*/
	delete from tilelifterneed where finish = 1 and DATEDIFF(CURRENT_DATE,create_time) >= 7;
END
;;
delimiter ;

-- ----------------------------
-- Event structure for DELETE_EVEN
-- ----------------------------
DROP EVENT IF EXISTS `DELETE_EVEN`;
delimiter ;;
CREATE EVENT `DELETE_EVEN`
ON SCHEDULE
EVERY '1' DAY STARTS '2021-06-30 01:00:00'
DO CALL DELETE_DATA()
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table config_tilelifter
-- ----------------------------
DROP TRIGGER IF EXISTS `update_good_change`;
delimiter ;;
CREATE TRIGGER `update_good_change` BEFORE UPDATE ON `config_tilelifter` FOR EACH ROW BEGIN
    IF
		(old.goods_id != new.goods_id) OR (old.pre_goodid != new.pre_goodid) 
		THEN
			SET new.good_change = 1;
	END IF;
END
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
