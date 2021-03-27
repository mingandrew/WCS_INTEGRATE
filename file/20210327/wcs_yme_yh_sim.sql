/*
 Navicat Premium Data Transfer

 Source Server         : TEST
 Source Server Type    : MySQL
 Source Server Version : 80016
 Source Host           : localhost:3306
 Source Schema         : wcs_yme_yh_sim

 Target Server Type    : MySQL
 Target Server Version : 80016
 File Encoding         : 65001

 Date: 27/03/2021 18:59:17
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
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area
-- ----------------------------
INSERT INTO `area` VALUES (1, '1', b'1', b'1', '1');

-- ----------------------------
-- Table structure for device
-- ----------------------------
DROP TABLE IF EXISTS `device`;
CREATE TABLE `device`  (
  `id` tinyint(3) UNSIGNED NOT NULL COMMENT '标识',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '类型',
  `type2` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '类型2',
  `enable` bit(1) NULL DEFAULT NULL COMMENT '可用',
  `memo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `area_id` int(10) UNSIGNED NULL DEFAULT NULL,
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车用/摆渡车对应的轨道',
  `r_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车用/摆渡车对应的轨道',
  `lastsite` smallint(3) UNSIGNED NULL DEFAULT NULL COMMENT '初始化',
  `out_dis` tinyint(4) NULL DEFAULT NULL COMMENT '距离外面【相叠砖机】',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of device
-- ----------------------------
INSERT INTO `device` VALUES (161, '2A01', 1, 1, b'1', '', 1, 1, NULL, NULL, NULL);
INSERT INTO `device` VALUES (162, '2A02', 1, 1, b'1', '', 1, 2, NULL, NULL, NULL);
INSERT INTO `device` VALUES (163, '2A03', 1, 1, b'1', '', 1, 3, NULL, NULL, NULL);
INSERT INTO `device` VALUES (177, '2B01', 3, 0, b'1', '', 1, 31, NULL, NULL, NULL);
INSERT INTO `device` VALUES (181, '2B05', 2, 0, b'1', '', 1, 33, NULL, NULL, NULL);
INSERT INTO `device` VALUES (193, '2C01', 4, 0, b'1', '', 1, NULL, NULL, 201, NULL);
INSERT INTO `device` VALUES (194, '2C02', 4, 0, b'1', '', 1, NULL, NULL, 204, NULL);
INSERT INTO `device` VALUES (195, '2C03', 4, 0, b'1', '', 1, NULL, NULL, 501, NULL);
INSERT INTO `device` VALUES (196, '2C04', 4, 0, b'1', '', 1, NULL, NULL, 502, NULL);
INSERT INTO `device` VALUES (197, '2C05', 4, 0, b'1', '', 1, NULL, NULL, 503, NULL);
INSERT INTO `device` VALUES (198, '2C06', 4, 0, b'1', '', 1, NULL, NULL, 504, NULL);
INSERT INTO `device` VALUES (199, '2C07', 4, 0, b'1', '', 1, NULL, NULL, 505, NULL);
INSERT INTO `device` VALUES (209, '2D01', 0, 2, b'1', '', 1, 8, 9, NULL, NULL);
INSERT INTO `device` VALUES (210, '2D11', 0, 2, b'1', '', 1, 8, 9, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of diction
-- ----------------------------
INSERT INTO `diction` VALUES (1, 0, 4, '上砖设置', b'1', b'1', b'1', 1);
INSERT INTO `diction` VALUES (2, 0, 4, '下砖设置', b'1', b'1', b'1', 1);

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
  `double_value` double(6, 2) UNSIGNED NULL DEFAULT NULL COMMENT '浮点类型',
  `uint_value` int(11) UNSIGNED NULL DEFAULT NULL,
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `updatetime` datetime(0) NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP(0),
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `dic_id_fk`(`diction_id`) USING BTREE,
  CONSTRAINT `dic_id_fk` FOREIGN KEY (`diction_id`) REFERENCES `diction` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of diction_dtl
-- ----------------------------
INSERT INTO `diction_dtl` VALUES (1, 1, 'UpFullCount', '满砖数量', 5, NULL, NULL, NULL, NULL, NULL, '2021-03-27 17:30:32');
INSERT INTO `diction_dtl` VALUES (2, 1, 'UpPiecesTime', '单片时间', 5, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (3, 1, 'StockOutPiceseDis', '相邻砖放差距时间', 21, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (4, 1, 'StockOutFullTime', '取砖最长时间', 222, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (5, 2, 'DownFullCount', '满砖数量', 5, NULL, NULL, NULL, NULL, NULL, '2021-03-27 17:32:24');
INSERT INTO `diction_dtl` VALUES (6, 2, 'DownPiecesTime', '单片时间', 5, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (7, 2, 'StockInPiceseDis', '相邻砖放差距时间', 21, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (8, 2, 'StockInFullTime', '放砖最长时间', 222, NULL, NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for ferry_pos
-- ----------------------------
DROP TABLE IF EXISTS `ferry_pos`;
CREATE TABLE `ferry_pos`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '设备ID',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '轨道ID',
  `type` tinyint(4) NULL DEFAULT NULL COMMENT '类型 1下砖 2 上砖',
  `ferry_code` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡编码',
  `ferry_pos` int(11) NULL DEFAULT NULL COMMENT '实际坐标',
  `isdownsite` bit(1) NULL DEFAULT NULL COMMENT '是否是下砖测',
  `ismin` bit(1) NULL DEFAULT NULL COMMENT '最小轨道',
  `ismax` bit(1) NULL DEFAULT NULL COMMENT '最大轨道',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fepos_traid_idx`(`track_id`) USING BTREE,
  INDEX `fepos_devid_idx`(`area_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 69 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ferry_pos
-- ----------------------------
INSERT INTO `ferry_pos` VALUES (1, 1, 0, 1, 101, 200, b'1', b'1', b'0');
INSERT INTO `ferry_pos` VALUES (2, 1, 0, 1, 102, 400, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (3, 1, 0, 1, 103, 700, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (4, 1, 0, 1, 201, 600, b'0', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (5, 1, 0, 1, 202, 900, b'0', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (6, 1, 0, 1, 203, 1100, b'0', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (7, 1, 0, 1, 204, 1300, b'0', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (8, 1, 0, 1, 205, 1500, b'0', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (9, 1, 0, 1, 206, 1700, b'0', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (10, 1, 0, 1, 207, 1900, b'0', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (11, 1, 0, 1, 208, 2100, b'0', b'0', b'1');
INSERT INTO `ferry_pos` VALUES (12, 1, 0, 2, 501, 500, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (13, 1, 0, 2, 502, 800, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (14, 1, 0, 2, 503, 1000, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (15, 1, 0, 2, 504, 1200, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (16, 1, 0, 2, 505, 1400, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (17, 1, 0, 2, 506, 1600, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (18, 1, 0, 2, 507, 1800, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (19, 1, 0, 2, 508, 2000, b'1', b'0', b'1');
INSERT INTO `ferry_pos` VALUES (20, 1, 0, 2, 520, 300, b'1', b'0', b'0');
INSERT INTO `ferry_pos` VALUES (21, 1, 0, 2, 521, 100, b'1', b'1', b'0');

-- ----------------------------
-- Table structure for track
-- ----------------------------
DROP TABLE IF EXISTS `track`;
CREATE TABLE `track`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `brother_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '相邻轨道ID',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `area_id` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域：过滤作用',
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '类型',
  `status` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '状态',
  `max_store` int(6) UNSIGNED NULL DEFAULT NULL COMMENT '最大存储数量',
  `rfid_1` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `rfid_2` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `memo` smallint(6) NULL DEFAULT NULL COMMENT '备注',
  `store_count` int(11) NULL DEFAULT NULL,
  `store_pos` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 73 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of track
-- ----------------------------
INSERT INTO `track` VALUES (1, 0, '2A01_轨道', 1, 1, 0, 1, 101, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (2, 0, '2A02_轨道', 1, 1, 0, 1, 102, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (3, 0, '2A03_轨道', 1, 1, 0, 1, 103, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (8, 0, '2D01_1_轨道', 1, 0, 0, 1, 520, 530, NULL, NULL, NULL);
INSERT INTO `track` VALUES (9, 0, '2D01_2_轨道', 1, 0, 0, 1, 521, 531, NULL, NULL, NULL);
INSERT INTO `track` VALUES (10, 18, '2_01_入', 1, 2, 0, 10, 201, 301, NULL, 0, 0);
INSERT INTO `track` VALUES (11, 19, '2_02_入', 1, 2, 0, 10, 202, 302, NULL, 0, 0);
INSERT INTO `track` VALUES (12, 20, '2_03_入', 1, 2, 0, 10, 203, 303, NULL, 0, 0);
INSERT INTO `track` VALUES (13, 21, '2_04_入', 1, 2, 0, 10, 204, 304, NULL, 0, 0);
INSERT INTO `track` VALUES (14, 22, '2_05_入', 1, 2, 0, 10, 205, 305, NULL, NULL, NULL);
INSERT INTO `track` VALUES (15, 23, '2_06_入', 1, 2, 0, 10, 206, 306, NULL, NULL, NULL);
INSERT INTO `track` VALUES (16, 24, '2_07_入', 1, 2, 0, 10, 207, 307, NULL, NULL, NULL);
INSERT INTO `track` VALUES (17, 25, '2_08_入', 1, 2, 0, 10, 208, 308, NULL, NULL, NULL);
INSERT INTO `track` VALUES (18, 10, '2_01_出', 1, 3, 2, 10, 501, 401, NULL, 5, 5);
INSERT INTO `track` VALUES (19, 11, '2_02_出', 1, 3, 2, 10, 502, 402, NULL, 4, 4);
INSERT INTO `track` VALUES (20, 12, '2_03_出', 1, 3, 2, 10, 503, 403, NULL, 3, 3);
INSERT INTO `track` VALUES (21, 13, '2_04_出', 1, 3, 2, 10, 504, 404, NULL, 2, 2);
INSERT INTO `track` VALUES (22, 14, '2_05_出', 1, 3, 0, 10, 505, 405, NULL, NULL, NULL);
INSERT INTO `track` VALUES (23, 15, '2_06_出', 1, 3, 0, 10, 506, 406, NULL, NULL, NULL);
INSERT INTO `track` VALUES (24, 16, '2_07_出', 1, 3, 0, 10, 507, 407, NULL, NULL, NULL);
INSERT INTO `track` VALUES (25, 17, '2_08_出', 1, 3, 0, 10, 508, 408, NULL, NULL, NULL);
INSERT INTO `track` VALUES (31, 0, '2B01_摆轨', 1, 5, 0, 1, 701, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (33, 0, '2B05_摆轨', 1, 6, 0, 1, 741, NULL, NULL, NULL, NULL);

SET FOREIGN_KEY_CHECKS = 1;
