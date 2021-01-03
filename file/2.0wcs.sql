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

 Date: 03/01/2021 09:51:23
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
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area
-- ----------------------------
INSERT INTO `area` VALUES (1, '1#', b'1', b'1', '1号线', 3, 0, 5);

-- ----------------------------
-- Table structure for area_device
-- ----------------------------
DROP TABLE IF EXISTS `area_device`;
CREATE TABLE `area_device`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
  `device_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '设备ID',
  `dev_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '设备类型1',
  `dev_type2` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '设备类型2',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `at_tile_id_fk`(`device_id`) USING BTREE,
  INDEX `at_area_id_fk`(`area_id`) USING BTREE,
  CONSTRAINT `at_area_id_fk` FOREIGN KEY (`area_id`) REFERENCES `area` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `at_tile_id_fk` FOREIGN KEY (`device_id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 20 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域内设备' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_device
-- ----------------------------
INSERT INTO `area_device` VALUES (1, 1, 1, 1, NULL);
INSERT INTO `area_device` VALUES (2, 1, 2, 1, NULL);
INSERT INTO `area_device` VALUES (3, 1, 3, 1, NULL);
INSERT INTO `area_device` VALUES (4, 1, 4, 1, NULL);
INSERT INTO `area_device` VALUES (5, 1, 5, 1, NULL);
INSERT INTO `area_device` VALUES (6, 1, 6, 1, NULL);
INSERT INTO `area_device` VALUES (7, 1, 7, 1, NULL);
INSERT INTO `area_device` VALUES (8, 1, 8, 1, NULL);
INSERT INTO `area_device` VALUES (9, 1, 9, 0, NULL);
INSERT INTO `area_device` VALUES (10, 1, 10, 0, NULL);
INSERT INTO `area_device` VALUES (11, 1, 11, 3, NULL);
INSERT INTO `area_device` VALUES (12, 1, 12, 3, NULL);
INSERT INTO `area_device` VALUES (13, 1, 13, 2, NULL);
INSERT INTO `area_device` VALUES (15, 1, 15, 4, NULL);
INSERT INTO `area_device` VALUES (16, 1, 16, 4, NULL);
INSERT INTO `area_device` VALUES (17, 1, 17, 4, NULL);
INSERT INTO `area_device` VALUES (18, 1, 18, 4, NULL);
INSERT INTO `area_device` VALUES (19, 1, 19, 4, NULL);

-- ----------------------------
-- Table structure for area_device_track
-- ----------------------------
DROP TABLE IF EXISTS `area_device_track`;
CREATE TABLE `area_device_track`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `area_id` int(11) UNSIGNED NOT NULL COMMENT '区域ID',
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
) ENGINE = InnoDB AUTO_INCREMENT = 577 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域内设备作业的轨道' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_device_track
-- ----------------------------
INSERT INTO `area_device_track` VALUES (1, 1, 11, 19, 1);
INSERT INTO `area_device_track` VALUES (2, 1, 11, 20, 2);
INSERT INTO `area_device_track` VALUES (3, 1, 11, 21, 3);
INSERT INTO `area_device_track` VALUES (4, 1, 11, 22, 4);
INSERT INTO `area_device_track` VALUES (5, 1, 11, 23, 5);
INSERT INTO `area_device_track` VALUES (6, 1, 11, 24, 6);
INSERT INTO `area_device_track` VALUES (7, 1, 11, 25, 7);
INSERT INTO `area_device_track` VALUES (8, 1, 11, 26, 8);
INSERT INTO `area_device_track` VALUES (9, 1, 11, 27, 9);
INSERT INTO `area_device_track` VALUES (10, 1, 11, 28, 10);
INSERT INTO `area_device_track` VALUES (11, 1, 11, 29, 11);
INSERT INTO `area_device_track` VALUES (12, 1, 11, 30, 12);
INSERT INTO `area_device_track` VALUES (13, 1, 11, 31, 13);
INSERT INTO `area_device_track` VALUES (14, 1, 11, 32, 14);
INSERT INTO `area_device_track` VALUES (15, 1, 11, 33, 15);
INSERT INTO `area_device_track` VALUES (16, 1, 11, 34, 16);
INSERT INTO `area_device_track` VALUES (17, 1, 11, 35, 17);
INSERT INTO `area_device_track` VALUES (18, 1, 11, 36, 18);
INSERT INTO `area_device_track` VALUES (19, 1, 11, 37, 19);
INSERT INTO `area_device_track` VALUES (20, 1, 11, 38, 20);
INSERT INTO `area_device_track` VALUES (21, 1, 11, 39, 21);
INSERT INTO `area_device_track` VALUES (22, 1, 11, 40, 22);
INSERT INTO `area_device_track` VALUES (23, 1, 11, 41, 23);
INSERT INTO `area_device_track` VALUES (24, 1, 11, 42, 24);
INSERT INTO `area_device_track` VALUES (27, 1, 11, 1, 27);
INSERT INTO `area_device_track` VALUES (28, 1, 11, 2, 28);
INSERT INTO `area_device_track` VALUES (29, 1, 11, 3, 29);
INSERT INTO `area_device_track` VALUES (30, 1, 11, 4, 30);
INSERT INTO `area_device_track` VALUES (31, 1, 11, 5, 31);
INSERT INTO `area_device_track` VALUES (32, 1, 11, 6, 32);
INSERT INTO `area_device_track` VALUES (33, 1, 11, 7, 33);
INSERT INTO `area_device_track` VALUES (34, 1, 11, 8, 34);
INSERT INTO `area_device_track` VALUES (45, 1, 12, 21, 3);
INSERT INTO `area_device_track` VALUES (46, 1, 12, 22, 4);
INSERT INTO `area_device_track` VALUES (47, 1, 12, 23, 5);
INSERT INTO `area_device_track` VALUES (48, 1, 12, 24, 6);
INSERT INTO `area_device_track` VALUES (49, 1, 12, 25, 7);
INSERT INTO `area_device_track` VALUES (50, 1, 12, 26, 8);
INSERT INTO `area_device_track` VALUES (51, 1, 12, 27, 9);
INSERT INTO `area_device_track` VALUES (52, 1, 12, 28, 10);
INSERT INTO `area_device_track` VALUES (53, 1, 12, 29, 11);
INSERT INTO `area_device_track` VALUES (54, 1, 12, 30, 12);
INSERT INTO `area_device_track` VALUES (55, 1, 12, 31, 13);
INSERT INTO `area_device_track` VALUES (56, 1, 12, 32, 14);
INSERT INTO `area_device_track` VALUES (57, 1, 12, 33, 15);
INSERT INTO `area_device_track` VALUES (58, 1, 12, 34, 16);
INSERT INTO `area_device_track` VALUES (59, 1, 12, 35, 17);
INSERT INTO `area_device_track` VALUES (60, 1, 12, 36, 18);
INSERT INTO `area_device_track` VALUES (61, 1, 12, 37, 19);
INSERT INTO `area_device_track` VALUES (62, 1, 12, 38, 20);
INSERT INTO `area_device_track` VALUES (63, 1, 12, 39, 21);
INSERT INTO `area_device_track` VALUES (64, 1, 12, 40, 22);
INSERT INTO `area_device_track` VALUES (65, 1, 12, 41, 23);
INSERT INTO `area_device_track` VALUES (66, 1, 12, 42, 24);
INSERT INTO `area_device_track` VALUES (67, 1, 12, 43, 25);
INSERT INTO `area_device_track` VALUES (68, 1, 12, 44, 26);
INSERT INTO `area_device_track` VALUES (77, 1, 12, 9, 35);
INSERT INTO `area_device_track` VALUES (78, 1, 12, 10, 36);
INSERT INTO `area_device_track` VALUES (79, 1, 12, 11, 37);
INSERT INTO `area_device_track` VALUES (80, 1, 12, 12, 38);
INSERT INTO `area_device_track` VALUES (81, 1, 12, 13, 39);
INSERT INTO `area_device_track` VALUES (82, 1, 12, 14, 40);
INSERT INTO `area_device_track` VALUES (83, 1, 12, 15, 41);
INSERT INTO `area_device_track` VALUES (84, 1, 12, 16, 42);
INSERT INTO `area_device_track` VALUES (87, 1, 13, 19, 1);
INSERT INTO `area_device_track` VALUES (88, 1, 13, 20, 2);
INSERT INTO `area_device_track` VALUES (89, 1, 13, 21, 3);
INSERT INTO `area_device_track` VALUES (90, 1, 13, 22, 4);
INSERT INTO `area_device_track` VALUES (91, 1, 13, 23, 5);
INSERT INTO `area_device_track` VALUES (92, 1, 13, 24, 6);
INSERT INTO `area_device_track` VALUES (93, 1, 13, 25, 7);
INSERT INTO `area_device_track` VALUES (94, 1, 13, 26, 8);
INSERT INTO `area_device_track` VALUES (95, 1, 13, 27, 9);
INSERT INTO `area_device_track` VALUES (96, 1, 13, 28, 10);
INSERT INTO `area_device_track` VALUES (97, 1, 13, 29, 11);
INSERT INTO `area_device_track` VALUES (98, 1, 13, 30, 12);
INSERT INTO `area_device_track` VALUES (99, 1, 13, 31, 13);
INSERT INTO `area_device_track` VALUES (100, 1, 13, 32, 14);
INSERT INTO `area_device_track` VALUES (101, 1, 13, 33, 15);
INSERT INTO `area_device_track` VALUES (102, 1, 13, 34, 16);
INSERT INTO `area_device_track` VALUES (103, 1, 13, 35, 17);
INSERT INTO `area_device_track` VALUES (104, 1, 13, 36, 18);
INSERT INTO `area_device_track` VALUES (105, 1, 13, 37, 19);
INSERT INTO `area_device_track` VALUES (106, 1, 13, 38, 20);
INSERT INTO `area_device_track` VALUES (107, 1, 13, 39, 21);
INSERT INTO `area_device_track` VALUES (108, 1, 13, 40, 22);
INSERT INTO `area_device_track` VALUES (109, 1, 13, 41, 23);
INSERT INTO `area_device_track` VALUES (110, 1, 13, 42, 24);
INSERT INTO `area_device_track` VALUES (111, 1, 13, 43, 25);
INSERT INTO `area_device_track` VALUES (112, 1, 13, 44, 26);
INSERT INTO `area_device_track` VALUES (113, 1, 13, 17, 27);
INSERT INTO `area_device_track` VALUES (114, 1, 13, 18, 28);
INSERT INTO `area_device_track` VALUES (117, 1, 14, 21, 3);
INSERT INTO `area_device_track` VALUES (118, 1, 14, 22, 4);
INSERT INTO `area_device_track` VALUES (119, 1, 14, 23, 5);
INSERT INTO `area_device_track` VALUES (120, 1, 14, 24, 6);
INSERT INTO `area_device_track` VALUES (121, 1, 14, 25, 7);
INSERT INTO `area_device_track` VALUES (122, 1, 14, 26, 8);
INSERT INTO `area_device_track` VALUES (123, 1, 14, 27, 9);
INSERT INTO `area_device_track` VALUES (124, 1, 14, 28, 10);
INSERT INTO `area_device_track` VALUES (125, 1, 14, 29, 11);
INSERT INTO `area_device_track` VALUES (126, 1, 14, 30, 12);
INSERT INTO `area_device_track` VALUES (127, 1, 14, 31, 13);
INSERT INTO `area_device_track` VALUES (128, 1, 14, 32, 14);
INSERT INTO `area_device_track` VALUES (129, 1, 14, 33, 15);
INSERT INTO `area_device_track` VALUES (130, 1, 14, 34, 16);
INSERT INTO `area_device_track` VALUES (131, 1, 14, 35, 17);
INSERT INTO `area_device_track` VALUES (132, 1, 14, 36, 18);
INSERT INTO `area_device_track` VALUES (133, 1, 14, 37, 19);
INSERT INTO `area_device_track` VALUES (134, 1, 14, 38, 20);
INSERT INTO `area_device_track` VALUES (135, 1, 14, 39, 21);
INSERT INTO `area_device_track` VALUES (136, 1, 14, 40, 22);
INSERT INTO `area_device_track` VALUES (137, 1, 14, 41, 23);
INSERT INTO `area_device_track` VALUES (138, 1, 14, 42, 24);
INSERT INTO `area_device_track` VALUES (139, 1, 14, 43, 25);
INSERT INTO `area_device_track` VALUES (140, 1, 14, 44, 26);
INSERT INTO `area_device_track` VALUES (141, 1, 14, 17, 27);
INSERT INTO `area_device_track` VALUES (142, 1, 14, 18, 28);
INSERT INTO `area_device_track` VALUES (143, 1, 13, 49, 29);
INSERT INTO `area_device_track` VALUES (144, 1, 13, 50, 30);
INSERT INTO `area_device_track` VALUES (317, 1, 1, 19, 1);
INSERT INTO `area_device_track` VALUES (318, 1, 1, 20, 2);
INSERT INTO `area_device_track` VALUES (319, 1, 1, 21, 3);
INSERT INTO `area_device_track` VALUES (320, 1, 1, 22, 4);
INSERT INTO `area_device_track` VALUES (321, 1, 1, 23, 5);
INSERT INTO `area_device_track` VALUES (322, 1, 1, 24, 6);
INSERT INTO `area_device_track` VALUES (323, 1, 1, 25, 7);
INSERT INTO `area_device_track` VALUES (324, 1, 1, 26, 8);
INSERT INTO `area_device_track` VALUES (325, 1, 1, 27, 9);
INSERT INTO `area_device_track` VALUES (326, 1, 1, 28, 10);
INSERT INTO `area_device_track` VALUES (327, 1, 1, 29, 11);
INSERT INTO `area_device_track` VALUES (328, 1, 1, 30, 12);
INSERT INTO `area_device_track` VALUES (329, 1, 1, 31, 13);
INSERT INTO `area_device_track` VALUES (330, 1, 1, 32, 14);
INSERT INTO `area_device_track` VALUES (331, 1, 1, 33, 15);
INSERT INTO `area_device_track` VALUES (332, 1, 1, 34, 16);
INSERT INTO `area_device_track` VALUES (333, 1, 1, 35, 17);
INSERT INTO `area_device_track` VALUES (334, 1, 1, 36, 18);
INSERT INTO `area_device_track` VALUES (335, 1, 1, 37, 19);
INSERT INTO `area_device_track` VALUES (336, 1, 1, 38, 20);
INSERT INTO `area_device_track` VALUES (337, 1, 1, 39, 21);
INSERT INTO `area_device_track` VALUES (338, 1, 1, 40, 22);
INSERT INTO `area_device_track` VALUES (339, 1, 1, 41, 23);
INSERT INTO `area_device_track` VALUES (340, 1, 1, 42, 24);
INSERT INTO `area_device_track` VALUES (343, 1, 2, 19, 1);
INSERT INTO `area_device_track` VALUES (344, 1, 2, 20, 2);
INSERT INTO `area_device_track` VALUES (345, 1, 2, 21, 3);
INSERT INTO `area_device_track` VALUES (346, 1, 2, 22, 4);
INSERT INTO `area_device_track` VALUES (347, 1, 2, 23, 5);
INSERT INTO `area_device_track` VALUES (348, 1, 2, 24, 6);
INSERT INTO `area_device_track` VALUES (349, 1, 2, 25, 7);
INSERT INTO `area_device_track` VALUES (350, 1, 2, 26, 8);
INSERT INTO `area_device_track` VALUES (351, 1, 2, 27, 9);
INSERT INTO `area_device_track` VALUES (352, 1, 2, 28, 10);
INSERT INTO `area_device_track` VALUES (353, 1, 2, 29, 11);
INSERT INTO `area_device_track` VALUES (354, 1, 2, 30, 12);
INSERT INTO `area_device_track` VALUES (355, 1, 2, 31, 13);
INSERT INTO `area_device_track` VALUES (356, 1, 2, 32, 14);
INSERT INTO `area_device_track` VALUES (357, 1, 2, 33, 15);
INSERT INTO `area_device_track` VALUES (358, 1, 2, 34, 16);
INSERT INTO `area_device_track` VALUES (359, 1, 2, 35, 17);
INSERT INTO `area_device_track` VALUES (360, 1, 2, 36, 18);
INSERT INTO `area_device_track` VALUES (361, 1, 2, 37, 19);
INSERT INTO `area_device_track` VALUES (362, 1, 2, 38, 20);
INSERT INTO `area_device_track` VALUES (363, 1, 2, 39, 21);
INSERT INTO `area_device_track` VALUES (364, 1, 2, 40, 22);
INSERT INTO `area_device_track` VALUES (365, 1, 2, 41, 23);
INSERT INTO `area_device_track` VALUES (366, 1, 2, 42, 24);
INSERT INTO `area_device_track` VALUES (369, 1, 3, 19, 1);
INSERT INTO `area_device_track` VALUES (370, 1, 3, 20, 2);
INSERT INTO `area_device_track` VALUES (371, 1, 3, 21, 3);
INSERT INTO `area_device_track` VALUES (372, 1, 3, 22, 4);
INSERT INTO `area_device_track` VALUES (373, 1, 3, 23, 5);
INSERT INTO `area_device_track` VALUES (374, 1, 3, 24, 6);
INSERT INTO `area_device_track` VALUES (375, 1, 3, 25, 7);
INSERT INTO `area_device_track` VALUES (376, 1, 3, 26, 8);
INSERT INTO `area_device_track` VALUES (377, 1, 3, 27, 9);
INSERT INTO `area_device_track` VALUES (378, 1, 3, 28, 10);
INSERT INTO `area_device_track` VALUES (379, 1, 3, 29, 11);
INSERT INTO `area_device_track` VALUES (380, 1, 3, 30, 12);
INSERT INTO `area_device_track` VALUES (381, 1, 3, 31, 13);
INSERT INTO `area_device_track` VALUES (382, 1, 3, 32, 14);
INSERT INTO `area_device_track` VALUES (383, 1, 3, 33, 15);
INSERT INTO `area_device_track` VALUES (384, 1, 3, 34, 16);
INSERT INTO `area_device_track` VALUES (385, 1, 3, 35, 17);
INSERT INTO `area_device_track` VALUES (386, 1, 3, 36, 18);
INSERT INTO `area_device_track` VALUES (387, 1, 3, 37, 19);
INSERT INTO `area_device_track` VALUES (388, 1, 3, 38, 20);
INSERT INTO `area_device_track` VALUES (389, 1, 3, 39, 21);
INSERT INTO `area_device_track` VALUES (390, 1, 3, 40, 22);
INSERT INTO `area_device_track` VALUES (391, 1, 3, 41, 23);
INSERT INTO `area_device_track` VALUES (392, 1, 3, 42, 24);
INSERT INTO `area_device_track` VALUES (395, 1, 4, 19, 1);
INSERT INTO `area_device_track` VALUES (396, 1, 4, 20, 2);
INSERT INTO `area_device_track` VALUES (397, 1, 4, 21, 3);
INSERT INTO `area_device_track` VALUES (398, 1, 4, 22, 4);
INSERT INTO `area_device_track` VALUES (399, 1, 4, 23, 5);
INSERT INTO `area_device_track` VALUES (400, 1, 4, 24, 6);
INSERT INTO `area_device_track` VALUES (401, 1, 4, 25, 7);
INSERT INTO `area_device_track` VALUES (402, 1, 4, 26, 8);
INSERT INTO `area_device_track` VALUES (403, 1, 4, 27, 9);
INSERT INTO `area_device_track` VALUES (404, 1, 4, 28, 10);
INSERT INTO `area_device_track` VALUES (405, 1, 4, 29, 11);
INSERT INTO `area_device_track` VALUES (406, 1, 4, 30, 12);
INSERT INTO `area_device_track` VALUES (407, 1, 4, 31, 13);
INSERT INTO `area_device_track` VALUES (408, 1, 4, 32, 14);
INSERT INTO `area_device_track` VALUES (409, 1, 4, 33, 15);
INSERT INTO `area_device_track` VALUES (410, 1, 4, 34, 16);
INSERT INTO `area_device_track` VALUES (411, 1, 4, 35, 17);
INSERT INTO `area_device_track` VALUES (412, 1, 4, 36, 18);
INSERT INTO `area_device_track` VALUES (413, 1, 4, 37, 19);
INSERT INTO `area_device_track` VALUES (414, 1, 4, 38, 20);
INSERT INTO `area_device_track` VALUES (415, 1, 4, 39, 21);
INSERT INTO `area_device_track` VALUES (416, 1, 4, 40, 22);
INSERT INTO `area_device_track` VALUES (417, 1, 4, 41, 23);
INSERT INTO `area_device_track` VALUES (418, 1, 4, 42, 24);
INSERT INTO `area_device_track` VALUES (423, 1, 5, 21, 24);
INSERT INTO `area_device_track` VALUES (424, 1, 5, 22, 23);
INSERT INTO `area_device_track` VALUES (425, 1, 5, 23, 22);
INSERT INTO `area_device_track` VALUES (426, 1, 5, 24, 21);
INSERT INTO `area_device_track` VALUES (427, 1, 5, 25, 20);
INSERT INTO `area_device_track` VALUES (428, 1, 5, 26, 19);
INSERT INTO `area_device_track` VALUES (429, 1, 5, 27, 18);
INSERT INTO `area_device_track` VALUES (430, 1, 5, 28, 17);
INSERT INTO `area_device_track` VALUES (431, 1, 5, 29, 16);
INSERT INTO `area_device_track` VALUES (432, 1, 5, 30, 15);
INSERT INTO `area_device_track` VALUES (433, 1, 5, 31, 14);
INSERT INTO `area_device_track` VALUES (434, 1, 5, 32, 13);
INSERT INTO `area_device_track` VALUES (435, 1, 5, 33, 12);
INSERT INTO `area_device_track` VALUES (436, 1, 5, 34, 11);
INSERT INTO `area_device_track` VALUES (437, 1, 5, 35, 10);
INSERT INTO `area_device_track` VALUES (438, 1, 5, 36, 9);
INSERT INTO `area_device_track` VALUES (439, 1, 5, 37, 8);
INSERT INTO `area_device_track` VALUES (440, 1, 5, 38, 7);
INSERT INTO `area_device_track` VALUES (441, 1, 5, 39, 6);
INSERT INTO `area_device_track` VALUES (442, 1, 5, 40, 5);
INSERT INTO `area_device_track` VALUES (443, 1, 5, 41, 4);
INSERT INTO `area_device_track` VALUES (444, 1, 5, 42, 3);
INSERT INTO `area_device_track` VALUES (445, 1, 5, 43, 2);
INSERT INTO `area_device_track` VALUES (446, 1, 5, 44, 1);
INSERT INTO `area_device_track` VALUES (449, 1, 6, 21, 24);
INSERT INTO `area_device_track` VALUES (450, 1, 6, 22, 23);
INSERT INTO `area_device_track` VALUES (451, 1, 6, 23, 22);
INSERT INTO `area_device_track` VALUES (452, 1, 6, 24, 21);
INSERT INTO `area_device_track` VALUES (453, 1, 6, 25, 20);
INSERT INTO `area_device_track` VALUES (454, 1, 6, 26, 19);
INSERT INTO `area_device_track` VALUES (455, 1, 6, 27, 18);
INSERT INTO `area_device_track` VALUES (456, 1, 6, 28, 17);
INSERT INTO `area_device_track` VALUES (457, 1, 6, 29, 16);
INSERT INTO `area_device_track` VALUES (458, 1, 6, 30, 15);
INSERT INTO `area_device_track` VALUES (459, 1, 6, 31, 14);
INSERT INTO `area_device_track` VALUES (460, 1, 6, 32, 13);
INSERT INTO `area_device_track` VALUES (461, 1, 6, 33, 12);
INSERT INTO `area_device_track` VALUES (462, 1, 6, 34, 11);
INSERT INTO `area_device_track` VALUES (463, 1, 6, 35, 10);
INSERT INTO `area_device_track` VALUES (464, 1, 6, 36, 9);
INSERT INTO `area_device_track` VALUES (465, 1, 6, 37, 8);
INSERT INTO `area_device_track` VALUES (466, 1, 6, 38, 7);
INSERT INTO `area_device_track` VALUES (467, 1, 6, 39, 6);
INSERT INTO `area_device_track` VALUES (468, 1, 6, 40, 5);
INSERT INTO `area_device_track` VALUES (469, 1, 6, 41, 4);
INSERT INTO `area_device_track` VALUES (470, 1, 6, 42, 3);
INSERT INTO `area_device_track` VALUES (471, 1, 6, 43, 2);
INSERT INTO `area_device_track` VALUES (472, 1, 6, 44, 1);
INSERT INTO `area_device_track` VALUES (475, 1, 7, 21, 24);
INSERT INTO `area_device_track` VALUES (476, 1, 7, 22, 23);
INSERT INTO `area_device_track` VALUES (477, 1, 7, 23, 22);
INSERT INTO `area_device_track` VALUES (478, 1, 7, 24, 21);
INSERT INTO `area_device_track` VALUES (479, 1, 7, 25, 20);
INSERT INTO `area_device_track` VALUES (480, 1, 7, 26, 19);
INSERT INTO `area_device_track` VALUES (481, 1, 7, 27, 18);
INSERT INTO `area_device_track` VALUES (482, 1, 7, 28, 17);
INSERT INTO `area_device_track` VALUES (483, 1, 7, 29, 16);
INSERT INTO `area_device_track` VALUES (484, 1, 7, 30, 15);
INSERT INTO `area_device_track` VALUES (485, 1, 7, 31, 14);
INSERT INTO `area_device_track` VALUES (486, 1, 7, 32, 13);
INSERT INTO `area_device_track` VALUES (487, 1, 7, 33, 12);
INSERT INTO `area_device_track` VALUES (488, 1, 7, 34, 11);
INSERT INTO `area_device_track` VALUES (489, 1, 7, 35, 10);
INSERT INTO `area_device_track` VALUES (490, 1, 7, 36, 9);
INSERT INTO `area_device_track` VALUES (491, 1, 7, 37, 8);
INSERT INTO `area_device_track` VALUES (492, 1, 7, 38, 7);
INSERT INTO `area_device_track` VALUES (493, 1, 7, 39, 6);
INSERT INTO `area_device_track` VALUES (494, 1, 7, 40, 5);
INSERT INTO `area_device_track` VALUES (495, 1, 7, 41, 4);
INSERT INTO `area_device_track` VALUES (496, 1, 7, 42, 3);
INSERT INTO `area_device_track` VALUES (497, 1, 7, 43, 2);
INSERT INTO `area_device_track` VALUES (498, 1, 7, 44, 1);
INSERT INTO `area_device_track` VALUES (501, 1, 8, 21, 24);
INSERT INTO `area_device_track` VALUES (502, 1, 8, 22, 23);
INSERT INTO `area_device_track` VALUES (503, 1, 8, 23, 22);
INSERT INTO `area_device_track` VALUES (504, 1, 8, 24, 21);
INSERT INTO `area_device_track` VALUES (505, 1, 8, 25, 20);
INSERT INTO `area_device_track` VALUES (506, 1, 8, 26, 19);
INSERT INTO `area_device_track` VALUES (507, 1, 8, 27, 18);
INSERT INTO `area_device_track` VALUES (508, 1, 8, 28, 17);
INSERT INTO `area_device_track` VALUES (509, 1, 8, 29, 16);
INSERT INTO `area_device_track` VALUES (510, 1, 8, 30, 15);
INSERT INTO `area_device_track` VALUES (511, 1, 8, 31, 14);
INSERT INTO `area_device_track` VALUES (512, 1, 8, 32, 13);
INSERT INTO `area_device_track` VALUES (513, 1, 8, 33, 12);
INSERT INTO `area_device_track` VALUES (514, 1, 8, 34, 11);
INSERT INTO `area_device_track` VALUES (515, 1, 8, 35, 10);
INSERT INTO `area_device_track` VALUES (516, 1, 8, 36, 9);
INSERT INTO `area_device_track` VALUES (517, 1, 8, 37, 8);
INSERT INTO `area_device_track` VALUES (518, 1, 8, 38, 7);
INSERT INTO `area_device_track` VALUES (519, 1, 8, 39, 6);
INSERT INTO `area_device_track` VALUES (520, 1, 8, 40, 5);
INSERT INTO `area_device_track` VALUES (521, 1, 8, 41, 4);
INSERT INTO `area_device_track` VALUES (522, 1, 8, 42, 3);
INSERT INTO `area_device_track` VALUES (523, 1, 8, 43, 2);
INSERT INTO `area_device_track` VALUES (524, 1, 8, 44, 1);
INSERT INTO `area_device_track` VALUES (525, 1, 9, 19, 1);
INSERT INTO `area_device_track` VALUES (526, 1, 9, 20, 2);
INSERT INTO `area_device_track` VALUES (527, 1, 9, 21, 3);
INSERT INTO `area_device_track` VALUES (528, 1, 9, 22, 4);
INSERT INTO `area_device_track` VALUES (529, 1, 9, 23, 5);
INSERT INTO `area_device_track` VALUES (530, 1, 9, 24, 6);
INSERT INTO `area_device_track` VALUES (531, 1, 9, 25, 7);
INSERT INTO `area_device_track` VALUES (532, 1, 9, 26, 8);
INSERT INTO `area_device_track` VALUES (533, 1, 9, 27, 9);
INSERT INTO `area_device_track` VALUES (534, 1, 9, 28, 10);
INSERT INTO `area_device_track` VALUES (535, 1, 9, 29, 11);
INSERT INTO `area_device_track` VALUES (536, 1, 9, 30, 12);
INSERT INTO `area_device_track` VALUES (537, 1, 9, 31, 13);
INSERT INTO `area_device_track` VALUES (538, 1, 9, 32, 14);
INSERT INTO `area_device_track` VALUES (539, 1, 9, 33, 15);
INSERT INTO `area_device_track` VALUES (540, 1, 9, 34, 16);
INSERT INTO `area_device_track` VALUES (541, 1, 9, 35, 17);
INSERT INTO `area_device_track` VALUES (542, 1, 9, 36, 18);
INSERT INTO `area_device_track` VALUES (543, 1, 9, 37, 19);
INSERT INTO `area_device_track` VALUES (544, 1, 9, 38, 20);
INSERT INTO `area_device_track` VALUES (545, 1, 9, 39, 21);
INSERT INTO `area_device_track` VALUES (546, 1, 9, 40, 22);
INSERT INTO `area_device_track` VALUES (547, 1, 9, 41, 23);
INSERT INTO `area_device_track` VALUES (548, 1, 9, 42, 24);
INSERT INTO `area_device_track` VALUES (549, 1, 9, 43, 25);
INSERT INTO `area_device_track` VALUES (550, 1, 9, 44, 26);
INSERT INTO `area_device_track` VALUES (551, 1, 10, 19, 1);
INSERT INTO `area_device_track` VALUES (552, 1, 10, 20, 2);
INSERT INTO `area_device_track` VALUES (553, 1, 10, 21, 3);
INSERT INTO `area_device_track` VALUES (554, 1, 10, 22, 4);
INSERT INTO `area_device_track` VALUES (555, 1, 10, 23, 5);
INSERT INTO `area_device_track` VALUES (556, 1, 10, 24, 6);
INSERT INTO `area_device_track` VALUES (557, 1, 10, 25, 7);
INSERT INTO `area_device_track` VALUES (558, 1, 10, 26, 8);
INSERT INTO `area_device_track` VALUES (559, 1, 10, 27, 9);
INSERT INTO `area_device_track` VALUES (560, 1, 10, 28, 10);
INSERT INTO `area_device_track` VALUES (561, 1, 10, 29, 11);
INSERT INTO `area_device_track` VALUES (562, 1, 10, 30, 12);
INSERT INTO `area_device_track` VALUES (563, 1, 10, 31, 13);
INSERT INTO `area_device_track` VALUES (564, 1, 10, 32, 14);
INSERT INTO `area_device_track` VALUES (565, 1, 10, 33, 15);
INSERT INTO `area_device_track` VALUES (566, 1, 10, 34, 16);
INSERT INTO `area_device_track` VALUES (567, 1, 10, 35, 17);
INSERT INTO `area_device_track` VALUES (568, 1, 10, 36, 18);
INSERT INTO `area_device_track` VALUES (569, 1, 10, 37, 19);
INSERT INTO `area_device_track` VALUES (570, 1, 10, 38, 20);
INSERT INTO `area_device_track` VALUES (571, 1, 10, 39, 21);
INSERT INTO `area_device_track` VALUES (572, 1, 10, 40, 22);
INSERT INTO `area_device_track` VALUES (573, 1, 10, 41, 23);
INSERT INTO `area_device_track` VALUES (574, 1, 10, 42, 24);
INSERT INTO `area_device_track` VALUES (575, 1, 10, 43, 25);
INSERT INTO `area_device_track` VALUES (576, 1, 10, 44, 26);

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
) ENGINE = InnoDB AUTO_INCREMENT = 51 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域内轨道' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_track
-- ----------------------------
INSERT INTO `area_track` VALUES (1, 1, 1, 1);
INSERT INTO `area_track` VALUES (2, 1, 2, 1);
INSERT INTO `area_track` VALUES (3, 1, 3, 1);
INSERT INTO `area_track` VALUES (4, 1, 4, 1);
INSERT INTO `area_track` VALUES (5, 1, 5, 1);
INSERT INTO `area_track` VALUES (6, 1, 6, 1);
INSERT INTO `area_track` VALUES (7, 1, 7, 1);
INSERT INTO `area_track` VALUES (8, 1, 8, 1);
INSERT INTO `area_track` VALUES (9, 1, 9, 1);
INSERT INTO `area_track` VALUES (10, 1, 10, 1);
INSERT INTO `area_track` VALUES (11, 1, 11, 1);
INSERT INTO `area_track` VALUES (12, 1, 12, 1);
INSERT INTO `area_track` VALUES (13, 1, 13, 1);
INSERT INTO `area_track` VALUES (14, 1, 14, 1);
INSERT INTO `area_track` VALUES (15, 1, 15, 1);
INSERT INTO `area_track` VALUES (16, 1, 16, 1);
INSERT INTO `area_track` VALUES (17, 1, 17, 0);
INSERT INTO `area_track` VALUES (18, 1, 18, 0);
INSERT INTO `area_track` VALUES (19, 1, 19, 4);
INSERT INTO `area_track` VALUES (20, 1, 20, 4);
INSERT INTO `area_track` VALUES (21, 1, 21, 4);
INSERT INTO `area_track` VALUES (22, 1, 22, 4);
INSERT INTO `area_track` VALUES (23, 1, 23, 4);
INSERT INTO `area_track` VALUES (24, 1, 24, 4);
INSERT INTO `area_track` VALUES (25, 1, 25, 4);
INSERT INTO `area_track` VALUES (26, 1, 26, 4);
INSERT INTO `area_track` VALUES (27, 1, 27, 4);
INSERT INTO `area_track` VALUES (28, 1, 28, 4);
INSERT INTO `area_track` VALUES (29, 1, 29, 4);
INSERT INTO `area_track` VALUES (30, 1, 30, 4);
INSERT INTO `area_track` VALUES (31, 1, 31, 4);
INSERT INTO `area_track` VALUES (32, 1, 32, 4);
INSERT INTO `area_track` VALUES (33, 1, 33, 4);
INSERT INTO `area_track` VALUES (34, 1, 34, 4);
INSERT INTO `area_track` VALUES (35, 1, 35, 4);
INSERT INTO `area_track` VALUES (36, 1, 36, 4);
INSERT INTO `area_track` VALUES (37, 1, 37, 4);
INSERT INTO `area_track` VALUES (38, 1, 38, 4);
INSERT INTO `area_track` VALUES (39, 1, 39, 4);
INSERT INTO `area_track` VALUES (40, 1, 40, 4);
INSERT INTO `area_track` VALUES (41, 1, 41, 4);
INSERT INTO `area_track` VALUES (42, 1, 42, 4);
INSERT INTO `area_track` VALUES (43, 1, 43, 4);
INSERT INTO `area_track` VALUES (44, 1, 44, 4);
INSERT INTO `area_track` VALUES (45, 1, 45, 5);
INSERT INTO `area_track` VALUES (46, 1, 46, 5);
INSERT INTO `area_track` VALUES (47, 1, 47, 6);
INSERT INTO `area_track` VALUES (48, 1, 48, 6);
INSERT INTO `area_track` VALUES (49, 1, 49, 0);
INSERT INTO `area_track` VALUES (50, 1, 50, 0);

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
  `goods_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '品种ID',
  `left_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上下砖机：左轨道ID',
  `right_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上下砖机：右轨道ID',
  `brother_dev_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '前设备ID[兄弟砖机ID]',
  `strategy_in` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '下砖策略',
  `strategy_out` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '上砖策略',
  `memo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `area` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域值用于过滤',
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '排序',
  `offlinetime` datetime(0) NULL DEFAULT NULL COMMENT '离线时间',
  `a_givemisstrack` bit(1) NULL DEFAULT NULL COMMENT '前进放货扫不到点',
  `a_takemisstrack` bit(1) NULL DEFAULT NULL COMMENT '后退取货扫不到点',
  `a_poweroff` bit(1) NULL DEFAULT NULL COMMENT '轨道掉电',
  `a_alert_track` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '所在轨道',
  `do_work` bit(1) NULL DEFAULT NULL COMMENT '开启作业',
  `work_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '作业类型\r\n砖机：0按规格 1按轨道',
  `ignorearea` bit(1) NULL DEFAULT NULL COMMENT '砖机上砖机是否忽略区域的规格',
  `last_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '砖机上次作业轨道',
  `old_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上一个品种',
  `pre_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '预设品种',
  `do_shift` bit(1) NULL DEFAULT NULL COMMENT '开启转产',
  `left_goods` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '左轨道品种',
  `right_goods` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '右轨道品种',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `dev_goods_id_fk`(`goods_id`) USING BTREE,
  INDEX `dev_ltrack_id_fk`(`left_track_id`) USING BTREE,
  INDEX `dev_rtrack_id_fk`(`right_track_id`) USING BTREE,
  CONSTRAINT `dev_goods_id_fk` FOREIGN KEY (`goods_id`) REFERENCES `goods` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `dev_ltrack_id_fk` FOREIGN KEY (`left_track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `dev_rtrack_id_fk` FOREIGN KEY (`right_track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 20 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '设备' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of device
-- ----------------------------
INSERT INTO `device` VALUES (1, 'A01', '192.168.0.31', 2000, 1, 2, b'1', 0, 0, 192, 1, 2, 0, 4, 0, '161', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (2, 'A02', '192.168.0.36', 2000, 1, 2, b'1', 0, 0, 193, 3, 4, 0, 4, 0, '162', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (3, 'A03', '192.168.0.41', 2000, 1, 2, b'1', 0, 0, 192, 5, 6, 0, 4, 0, '163', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (4, 'A04', '192.168.0.46', 2000, 1, 2, b'1', 0, 0, 192, 7, 8, 0, 4, 0, '164', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (5, 'A05', '192.168.0.51', 2000, 1, 2, b'1', 0, 0, 194, 9, 10, 0, 4, 0, '165', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (6, 'A06', '192.168.0.56', 2000, 1, 2, b'1', 0, 0, 194, 11, 12, 0, 4, 0, '166', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (7, 'A07', '192.168.0.61', 2000, 1, 2, b'1', 0, 0, 195, 13, 14, 0, 4, 0, '167', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (8, 'A08', '192.168.0.66', 2000, 1, 2, b'1', 0, 0, 194, 15, 16, 0, 4, 0, '168', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (9, 'D01', '192.168.0.81', 2000, 0, 2, b'1', 0, 0, 192, 17, 18, 0, 0, 1, '209', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (10, 'D02', '192.168.0.86', 2000, 0, 2, b'1', 0, 0, 188, 49, 50, 0, 0, 1, '210', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (11, 'B01', '192.168.0.131', 2000, 3, 0, b'1', 0, 0, NULL, 45, NULL, 0, 0, 0, '177', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (12, 'B02', '192.168.0.132', 2000, 3, 0, b'1', 0, 0, NULL, 46, NULL, 0, 0, 0, '178', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (13, 'B05', '192.168.0.135', 2000, 2, 0, b'1', 0, 0, NULL, 47, NULL, 0, 0, 0, '181', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (14, '1_B6', NULL, NULL, 9, 0, b'0', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (15, 'C01', '192.168.0.151', 2000, 4, 0, b'1', 0, 0, NULL, NULL, NULL, 0, 0, 0, '193', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (16, 'C02', '192.168.0.152', 2000, 4, 0, b'1', 0, 0, NULL, NULL, NULL, 0, 0, 0, '194', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (17, 'C03', '192.168.0.153', 2000, 4, 0, b'1', 0, 0, NULL, NULL, NULL, 0, 0, 0, '195', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (18, 'C04', '192.168.0.154', 2000, 4, 0, b'1', 0, 0, NULL, NULL, NULL, 0, 0, 0, '196', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);
INSERT INTO `device` VALUES (19, 'C05', '192.168.0.155', 2000, 4, 0, b'1', 0, 0, NULL, NULL, NULL, 0, 0, 0, '197', 1, NULL, NULL, NULL, NULL, NULL, NULL, b'1', 0, NULL, 0, 0, 0, b'0', 1, 1);

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
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '字典' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of diction
-- ----------------------------
INSERT INTO `diction` VALUES (1, 4, 4, '序列号生成', b'0', b'0', b'0', 1);
INSERT INTO `diction` VALUES (2, 0, 1, '任务开关', b'1', b'1', b'1', 1);
INSERT INTO `diction` VALUES (3, 0, 2, '警告信息', b'1', b'1', b'1', 1);
INSERT INTO `diction` VALUES (4, 0, 0, '最小存放时间', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (5, 0, 0, '安全距离', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (6, 0, 0, '版本信息', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (7, 0, 0, '转产差值', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (8, 0, 1, '配置开关', b'0', b'1', b'0', 100);

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
  `updatetime` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `dic_id_fk`(`diction_id`) USING BTREE,
  CONSTRAINT `dic_id_fk` FOREIGN KEY (`diction_id`) REFERENCES `diction` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 61 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of diction_dtl
-- ----------------------------
INSERT INTO `diction_dtl` VALUES (1, 1, 'NewStockId', '生成库存ID', NULL, NULL, '', NULL, 5323, NULL, '2020-12-31 16:59:49');
INSERT INTO `diction_dtl` VALUES (2, 1, 'NewTranId', '生成交易ID', NULL, NULL, '', NULL, 8294, NULL, '2020-12-17 10:11:46');
INSERT INTO `diction_dtl` VALUES (3, 1, 'NewWarnId', '生成警告ID', NULL, NULL, '', NULL, 6799, NULL, '2020-12-21 12:05:24');
INSERT INTO `diction_dtl` VALUES (4, 1, 'NewGoodId', '生成品种ID', NULL, NULL, '', NULL, 198, NULL, '2020-12-17 06:38:23');
INSERT INTO `diction_dtl` VALUES (5, 2, 'Area1Down', '1号线下砖', NULL, b'0', '', NULL, NULL, NULL, '2020-12-31 16:58:51');
INSERT INTO `diction_dtl` VALUES (6, 2, 'Area1Up', '1号线上砖', NULL, b'0', '', NULL, NULL, NULL, '2020-12-31 16:58:52');
INSERT INTO `diction_dtl` VALUES (20, 3, 'DeviceOffline', '设备离线提示', NULL, NULL, '设备离线', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (21, 3, 'TrackFullButNoneStock', '满砖无库存', NULL, NULL, '满砖无库存', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (22, 3, 'CarrierLoadSortTask', '小车倒库中但是小车有货', NULL, NULL, '小车倒库中但是小车有货', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (23, 3, 'CarrierLoadNotSortTask', '小车倒库中任务清除', NULL, NULL, '小车倒库中任务清除', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (24, 3, 'ReadConBreakenCheckWire', '阅读器断开[检查连接线]', NULL, NULL, 'RFID阅读器断开[检查连接线]', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (25, 3, 'StoreSlowOverTimeCheckLight', '前进储砖减速超时，检查定位光电', NULL, NULL, '前进储砖减速超时，检查定位光电', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (26, 3, 'FrontAvoidAlert', '前防撞触发', NULL, NULL, '前防撞触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (27, 3, 'BackAvoidAlert', '后防撞触发', NULL, NULL, '后防撞触发', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (28, 3, 'FunctinSwitchOverTime', '功能 切换超时', NULL, NULL, '功能 切换超时', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (29, 3, 'BackTakeOverTime', '后退取砖任务超时', NULL, NULL, '后退取砖任务超时', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (30, 3, 'FrontGiveOverTime', '前进放砖任务超时', NULL, NULL, '前进放砖任务超时', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (31, 3, 'FrontPointOverTime', '前进至点任务超时', NULL, NULL, '前进至点任务超时', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (32, 3, 'BackPointOverTime', '后退至点任务超时', NULL, NULL, '后退至点任务超时', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (33, 3, 'Back2FerryOverTime', '后退至摆渡任务超时', NULL, NULL, '后退至摆渡任务超时', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (34, 3, 'Front2FerryOverTime', '前进至摆渡任务超时', NULL, NULL, '前进至摆渡任务超时', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (35, 3, 'GoUpOverTime', '上升超时', NULL, NULL, '上升超时，检查上到位信号', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (36, 3, 'GoDownOverTime', '下降超时', NULL, NULL, '下降超时，检查下到位信号', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (37, 3, 'BackTakeCannotDo', '后退取砖条件不满足', NULL, NULL, '后退取砖条件不满足[摆渡上，下位，无砖]', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (38, 3, 'FrontGiveCannotDo', '前进放砖条件不满足', NULL, NULL, '前进放砖条件不满足[摆渡上，上位，有砖]', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (39, 3, 'Back2FerryCannotDo', '后退至摆渡条件不满足', NULL, NULL, '后退至摆渡条件不满足', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (40, 3, 'Front2FerryCannotDo', '前进至摆渡条件不满足', NULL, NULL, '前进至摆渡条件不满足', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (41, 3, 'Back2SortCannotDo', '后退至轨道倒库条件不满足', NULL, NULL, '后退倒库条件不满足[下位,无砖,轨道头]', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (42, 3, 'Front2PointCannotDo', '前进至点条件不满足', NULL, NULL, '前进至点条件不满足，是否在轨道前进方向轨道头部？', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (43, 3, 'Back2PointCannotDo', '后退至点条件不满足', NULL, NULL, '后退至点条件不满足，是否在轨道后退方向轨道头部？', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (44, 3, 'NotGoodToGoUp', '无砖不执行上升', NULL, NULL, '无砖不执行上升', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (45, 3, 'SortTaskOverTime', '倒库超时', NULL, NULL, '倒库超时', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (46, 3, 'TileNoneStrategy', '砖机没有设置策略', NULL, NULL, '砖机没有设置策略', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (47, 3, 'CarrierFullSignalFullNotOnStoreTrack', '小车满砖信号不在储砖轨道', NULL, NULL, '小车满砖信号不在储砖轨道', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (48, 3, 'CarrierGiveMissTrack', '前进放货没扫到地标', NULL, NULL, '前进放货没扫到地标,手动下降放货，移回轨道头', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (49, 3, 'DownTileHaveNotTrackToStore', '砖机找不到合适轨道存砖', NULL, NULL, '砖机找不到合适轨道存砖', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (50, 3, 'UpTileHaveNotStockToOut', '砖机找不到合适库存出库', NULL, NULL, '砖机找不到合适库存出库', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (51, 3, 'TrackEarlyFull', '请检查轨道是否提前满砖了', NULL, NULL, '请检查轨道是否提前满砖了', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (52, 4, 'MinStockTime', '最小存放时间(小时)', 0, NULL, NULL, NULL, NULL, NULL, '2020-11-18 21:26:18');
INSERT INTO `diction_dtl` VALUES (53, 5, 'FerryAvoidNumber', '摆渡车(轨道数)', 2, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (54, 3, 'UpTileHaveNoTrackToOut', '砖机找不到合适轨道上砖', NULL, NULL, '砖机找不到合适轨道上砖', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (57, 6, 'PDA_INIT_VERSION', 'PDA基础字典版本', 9, NULL, '', NULL, NULL, NULL, '2020-12-17 14:00:05');
INSERT INTO `diction_dtl` VALUES (58, 6, 'PDA_GOOD_VERSION', 'PDA规格字典版本', 5, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (59, 7, 'TileLifterShiftCount', '下砖机转产差值(层数)', 99, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (60, 8, 'UserLoginFunction', 'PDA登陆功能开关', NULL, b'1', 'PDA登陆功能开关', NULL, NULL, NULL, '2020-12-18 11:20:06');

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
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fepos_traid_idx`(`track_id`) USING BTREE,
  INDEX `fepos_devid_idx`(`device_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 155 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '摆渡对位' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ferry_pos
-- ----------------------------
INSERT INTO `ferry_pos` VALUES (1, 19, 13, 501, -6908);
INSERT INTO `ferry_pos` VALUES (2, 20, 13, 502, -6358);
INSERT INTO `ferry_pos` VALUES (3, 21, 13, 503, -5821);
INSERT INTO `ferry_pos` VALUES (4, 22, 13, 504, -5249);
INSERT INTO `ferry_pos` VALUES (5, 23, 13, 505, -4724);
INSERT INTO `ferry_pos` VALUES (6, 24, 13, 506, -4180);
INSERT INTO `ferry_pos` VALUES (7, 25, 13, 507, -3641);
INSERT INTO `ferry_pos` VALUES (8, 26, 13, 508, -3087);
INSERT INTO `ferry_pos` VALUES (9, 27, 13, 509, -2539);
INSERT INTO `ferry_pos` VALUES (10, 28, 13, 510, -1996);
INSERT INTO `ferry_pos` VALUES (11, 29, 13, 511, -916);
INSERT INTO `ferry_pos` VALUES (12, 30, 13, 512, -369);
INSERT INTO `ferry_pos` VALUES (13, 31, 13, 513, 178);
INSERT INTO `ferry_pos` VALUES (14, 32, 13, 514, 725);
INSERT INTO `ferry_pos` VALUES (15, 33, 13, 515, 1268);
INSERT INTO `ferry_pos` VALUES (16, 34, 13, 516, 1814);
INSERT INTO `ferry_pos` VALUES (17, 35, 13, 517, 2359);
INSERT INTO `ferry_pos` VALUES (18, 36, 13, 518, 2899);
INSERT INTO `ferry_pos` VALUES (19, 37, 13, 519, 3447);
INSERT INTO `ferry_pos` VALUES (20, 38, 13, 520, 3991);
INSERT INTO `ferry_pos` VALUES (21, 39, 13, 521, 4538);
INSERT INTO `ferry_pos` VALUES (22, 40, 13, 522, 5084);
INSERT INTO `ferry_pos` VALUES (23, 41, 13, 523, 5631);
INSERT INTO `ferry_pos` VALUES (24, 42, 13, 524, 6176);
INSERT INTO `ferry_pos` VALUES (25, 17, 13, 601, -5001);
INSERT INTO `ferry_pos` VALUES (26, 18, 13, 602, -3745);
INSERT INTO `ferry_pos` VALUES (27, 21, 14, 503, 0);
INSERT INTO `ferry_pos` VALUES (28, 22, 14, 504, 0);
INSERT INTO `ferry_pos` VALUES (29, 23, 14, 505, 0);
INSERT INTO `ferry_pos` VALUES (30, 24, 14, 506, 0);
INSERT INTO `ferry_pos` VALUES (31, 25, 14, 507, 0);
INSERT INTO `ferry_pos` VALUES (32, 26, 14, 508, 0);
INSERT INTO `ferry_pos` VALUES (33, 27, 14, 509, 0);
INSERT INTO `ferry_pos` VALUES (34, 28, 14, 510, 0);
INSERT INTO `ferry_pos` VALUES (35, 29, 14, 511, 0);
INSERT INTO `ferry_pos` VALUES (36, 30, 14, 512, 0);
INSERT INTO `ferry_pos` VALUES (37, 31, 14, 513, 0);
INSERT INTO `ferry_pos` VALUES (38, 32, 14, 514, 0);
INSERT INTO `ferry_pos` VALUES (39, 33, 14, 515, 0);
INSERT INTO `ferry_pos` VALUES (40, 34, 14, 516, 0);
INSERT INTO `ferry_pos` VALUES (41, 35, 14, 517, 0);
INSERT INTO `ferry_pos` VALUES (42, 36, 14, 518, 0);
INSERT INTO `ferry_pos` VALUES (43, 37, 14, 519, 0);
INSERT INTO `ferry_pos` VALUES (44, 38, 14, 520, 0);
INSERT INTO `ferry_pos` VALUES (45, 39, 14, 521, 0);
INSERT INTO `ferry_pos` VALUES (46, 40, 14, 522, 0);
INSERT INTO `ferry_pos` VALUES (47, 41, 14, 523, 0);
INSERT INTO `ferry_pos` VALUES (48, 42, 14, 524, 0);
INSERT INTO `ferry_pos` VALUES (49, 43, 14, 525, 0);
INSERT INTO `ferry_pos` VALUES (50, 44, 14, 526, 0);
INSERT INTO `ferry_pos` VALUES (51, 17, 14, 601, 0);
INSERT INTO `ferry_pos` VALUES (52, 18, 14, 602, 0);
INSERT INTO `ferry_pos` VALUES (53, 19, 11, 201, -3912);
INSERT INTO `ferry_pos` VALUES (54, 20, 11, 202, -3352);
INSERT INTO `ferry_pos` VALUES (55, 21, 11, 203, -2803);
INSERT INTO `ferry_pos` VALUES (56, 22, 11, 204, -2256);
INSERT INTO `ferry_pos` VALUES (57, 23, 11, 205, -1713);
INSERT INTO `ferry_pos` VALUES (58, 24, 11, 206, -1168);
INSERT INTO `ferry_pos` VALUES (59, 25, 11, 207, -626);
INSERT INTO `ferry_pos` VALUES (60, 26, 11, 208, -82);
INSERT INTO `ferry_pos` VALUES (61, 27, 11, 209, 463);
INSERT INTO `ferry_pos` VALUES (62, 28, 11, 210, 1007);
INSERT INTO `ferry_pos` VALUES (63, 29, 11, 211, 2107);
INSERT INTO `ferry_pos` VALUES (64, 30, 11, 212, 2658);
INSERT INTO `ferry_pos` VALUES (65, 31, 11, 213, 3202);
INSERT INTO `ferry_pos` VALUES (66, 32, 11, 214, 3751);
INSERT INTO `ferry_pos` VALUES (67, 33, 11, 215, 4297);
INSERT INTO `ferry_pos` VALUES (68, 34, 11, 216, 4840);
INSERT INTO `ferry_pos` VALUES (69, 35, 11, 217, 5392);
INSERT INTO `ferry_pos` VALUES (70, 36, 11, 218, 5937);
INSERT INTO `ferry_pos` VALUES (71, 37, 11, 219, 6488);
INSERT INTO `ferry_pos` VALUES (72, 38, 11, 220, 7032);
INSERT INTO `ferry_pos` VALUES (73, 39, 11, 221, 7579);
INSERT INTO `ferry_pos` VALUES (74, 40, 11, 222, 8121);
INSERT INTO `ferry_pos` VALUES (75, 41, 11, 223, 8670);
INSERT INTO `ferry_pos` VALUES (76, 42, 11, 224, 9216);
INSERT INTO `ferry_pos` VALUES (77, 1, 11, 101, -2971);
INSERT INTO `ferry_pos` VALUES (78, 2, 11, 102, -2415);
INSERT INTO `ferry_pos` VALUES (79, 3, 11, 103, -1229);
INSERT INTO `ferry_pos` VALUES (80, 4, 11, 104, -674);
INSERT INTO `ferry_pos` VALUES (81, 5, 11, 105, 444);
INSERT INTO `ferry_pos` VALUES (82, 6, 11, 106, 999);
INSERT INTO `ferry_pos` VALUES (83, 7, 11, 107, 2521);
INSERT INTO `ferry_pos` VALUES (84, 8, 11, 108, 3071);
INSERT INTO `ferry_pos` VALUES (85, 9, 11, 109, 4257);
INSERT INTO `ferry_pos` VALUES (86, 10, 11, 110, 4814);
INSERT INTO `ferry_pos` VALUES (87, 11, 11, 111, 6009);
INSERT INTO `ferry_pos` VALUES (88, 12, 11, 112, 6568);
INSERT INTO `ferry_pos` VALUES (89, 21, 12, 203, -10118);
INSERT INTO `ferry_pos` VALUES (90, 22, 12, 204, -9572);
INSERT INTO `ferry_pos` VALUES (91, 23, 12, 205, -9027);
INSERT INTO `ferry_pos` VALUES (92, 24, 12, 206, -8481);
INSERT INTO `ferry_pos` VALUES (93, 25, 12, 207, -7938);
INSERT INTO `ferry_pos` VALUES (94, 26, 12, 208, -7394);
INSERT INTO `ferry_pos` VALUES (95, 27, 12, 209, -6850);
INSERT INTO `ferry_pos` VALUES (96, 28, 12, 210, -6302);
INSERT INTO `ferry_pos` VALUES (97, 29, 12, 211, -5203);
INSERT INTO `ferry_pos` VALUES (98, 30, 12, 212, -4665);
INSERT INTO `ferry_pos` VALUES (99, 31, 12, 213, -4121);
INSERT INTO `ferry_pos` VALUES (100, 32, 12, 214, -3587);
INSERT INTO `ferry_pos` VALUES (101, 33, 12, 215, -3023);
INSERT INTO `ferry_pos` VALUES (102, 34, 12, 216, -2479);
INSERT INTO `ferry_pos` VALUES (103, 35, 12, 217, -1929);
INSERT INTO `ferry_pos` VALUES (104, 36, 12, 218, -1386);
INSERT INTO `ferry_pos` VALUES (105, 37, 12, 219, -845);
INSERT INTO `ferry_pos` VALUES (106, 38, 12, 220, -291);
INSERT INTO `ferry_pos` VALUES (107, 39, 12, 221, 252);
INSERT INTO `ferry_pos` VALUES (108, 40, 12, 222, 795);
INSERT INTO `ferry_pos` VALUES (109, 41, 12, 223, 1344);
INSERT INTO `ferry_pos` VALUES (110, 42, 12, 224, 1882);
INSERT INTO `ferry_pos` VALUES (111, 43, 12, 225, 2428);
INSERT INTO `ferry_pos` VALUES (112, 44, 12, 226, 2972);
INSERT INTO `ferry_pos` VALUES (113, 5, 12, 105, -6870);
INSERT INTO `ferry_pos` VALUES (114, 6, 12, 106, -6314);
INSERT INTO `ferry_pos` VALUES (115, 7, 12, 107, -4818);
INSERT INTO `ferry_pos` VALUES (116, 8, 12, 108, -4256);
INSERT INTO `ferry_pos` VALUES (117, 9, 12, 109, -3066);
INSERT INTO `ferry_pos` VALUES (118, 10, 12, 110, -2508);
INSERT INTO `ferry_pos` VALUES (119, 11, 12, 111, -1319);
INSERT INTO `ferry_pos` VALUES (120, 12, 12, 112, -767);
INSERT INTO `ferry_pos` VALUES (121, 13, 12, 113, 427);
INSERT INTO `ferry_pos` VALUES (122, 14, 12, 114, 980);
INSERT INTO `ferry_pos` VALUES (123, 15, 12, 115, 2168);
INSERT INTO `ferry_pos` VALUES (124, 16, 12, 116, 2729);
INSERT INTO `ferry_pos` VALUES (125, 49, 13, 603, 839);
INSERT INTO `ferry_pos` VALUES (126, 50, 13, 604, 2095);
INSERT INTO `ferry_pos` VALUES (127, 13, 11, 113, 7756);
INSERT INTO `ferry_pos` VALUES (128, 14, 11, 114, 8318);
INSERT INTO `ferry_pos` VALUES (129, 4, 12, 104, -7991);
INSERT INTO `ferry_pos` VALUES (130, 3, 12, 103, -8545);
INSERT INTO `ferry_pos` VALUES (131, 2, 12, 102, -9734);
INSERT INTO `ferry_pos` VALUES (132, 1, 12, 101, -10293);
INSERT INTO `ferry_pos` VALUES (133, 43, 13, 525, 6723);
INSERT INTO `ferry_pos` VALUES (134, 44, 13, 526, 7271);
INSERT INTO `ferry_pos` VALUES (149, 43, 11, 225, 0);
INSERT INTO `ferry_pos` VALUES (150, 44, 11, 226, 0);
INSERT INTO `ferry_pos` VALUES (151, 15, 11, 115, 0);
INSERT INTO `ferry_pos` VALUES (152, 16, 11, 116, 0);
INSERT INTO `ferry_pos` VALUES (153, 19, 12, 201, 0);
INSERT INTO `ferry_pos` VALUES (154, 20, 12, 202, 0);

-- ----------------------------
-- Table structure for goods
-- ----------------------------
DROP TABLE IF EXISTS `goods`;
CREATE TABLE `goods`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `area_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '区域',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '品种名称',
  `color` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '色号',
  `width` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '宽',
  `length` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '长',
  `oversize` bit(1) NULL DEFAULT NULL COMMENT '是否超限',
  `stack` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '垛数',
  `pieces` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '满砖数',
  `carriertype` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '运输车类型',
  `memo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `updatetime` datetime(0) NULL DEFAULT NULL,
  `minstack` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '最少托数',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 198 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '品种' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of goods
-- ----------------------------
INSERT INTO `goods` VALUES (33, 1, '881062', '881062', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (35, 1, '88103', '88103', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (36, 1, '8208', '8208', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (37, 1, '88307', '88307', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (38, 1, '88306', '88306', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (39, 1, '88711', '88711', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (40, 1, '88112', '88112', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (49, 1, '1:800x800-88116', '88116', 800, 800, b'0', 4, 51, 0, '', '2020-12-15 09:32:33', NULL);
INSERT INTO `goods` VALUES (50, 1, '88116一级', '88116一级', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (51, 1, '88311', '88311', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (60, 1, '3H092色', '3H092色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (61, 1, '5833', '5833', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (66, 1, '5833二色', '5833二色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (72, 1, '88311二色', '88311二色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (76, 1, '8106   3色', '8106  3色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (87, 1, '8812', '8812', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (100, 1, '88112L2色', '88112L2色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (113, 1, '88108  1色', '88108  1色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (164, 1, '6802一级', '6802一级', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (174, 1, '6859   一色', '6859   一色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (175, 1, '6859      一级', '6859    一级', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (176, 1, '6805一色', '6805一色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (177, 1, '6805一级', '6805一级', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (178, 1, '6808一色', '6808一色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (179, 1, '1:800x800-6808二色', '6808二色', 800, 800, b'0', 4, 51, 0, '', '2020-12-16 16:29:08', NULL);
INSERT INTO `goods` VALUES (180, 1, '6805二色', '6805二色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (181, 1, '88306一色', '88306一色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (182, 1, '88306一级', '88306一级', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (183, 1, '6859一色', '6859一色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (184, 1, '1:800x800-6808二色', '6808二色', 800, 800, b'0', 4, 51, 0, '', '2020-12-16 16:23:31', NULL);
INSERT INTO `goods` VALUES (185, 1, '6808二色一级', '6808二色一级', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (186, 1, '12702  一色', '12702  一色', 600, 1200, b'0', 3, 54, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (187, 1, '12702一级', '12702一级', 600, 1200, b'0', 3, 54, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (188, 1, '1.2*800-12702 一色', '1.2*800-12702 一色', 600, 1200, b'0', 3, 54, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (192, 1, '88702     一色', '88702   一色', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (193, 1, '88702    一级', '88702    一级', 800, 800, b'0', 4, 51, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (194, 1, '1.2*600-12702  二色', '1.2*600-12702  二色', 600, 1200, b'0', 3, 54, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (195, 1, '1.2*600-12702  二色一级', '1.2*600-12702  二色一级', 600, 1200, b'0', 3, 54, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (196, 1, '1.2*600-12702  一色', '1.2*600-12702  一色', 600, 1200, b'0', 3, 54, 0, '', NULL, NULL);
INSERT INTO `goods` VALUES (197, 1, '1.2*600-12702  一色一级', '1.2*600-12702  一色一级', 600, 1200, b'0', 3, 54, 0, '', NULL, NULL);

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
  PRIMARY KEY (`rfid`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '平板' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of rf_client
-- ----------------------------
INSERT INTO `rf_client` VALUES ('66666368200988122', '', '192.168.0.235:51742', '2020-12-17 17:08:56', '2020-12-18 11:30:15');
INSERT INTO `rf_client` VALUES ('861563040009909', '', '192.168.0.3:54917', '2020-11-18 14:09:19', '2020-12-16 17:33:05');
INSERT INTO `rf_client` VALUES ('861563040009941', '', '192.168.0.4:57487', '2020-11-18 14:10:16', '2020-12-16 15:56:54');

-- ----------------------------
-- Table structure for stock
-- ----------------------------
DROP TABLE IF EXISTS `stock`;
CREATE TABLE `stock`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '标识',
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
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `sto_goods_id_fk`(`goods_id`) USING BTREE,
  INDEX `sto_track_id_fk`(`track_id`) USING BTREE,
  CONSTRAINT `sto_goods_id_fk` FOREIGN KEY (`goods_id`) REFERENCES `goods` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `sto_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '库存' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock
-- ----------------------------
INSERT INTO `stock` VALUES (833, 33, 4, 204, 47, '2020-12-02 16:03:50', 2, 0, 7, 1, 6);
INSERT INTO `stock` VALUES (893, 35, 4, 204, 45, '2020-12-02 19:26:10', 2, 1, 2, 1, 5);
INSERT INTO `stock` VALUES (958, 36, 4, 204, 46, '2020-12-03 03:21:31', 0, 0, 7, 1, 5);
INSERT INTO `stock` VALUES (960, 36, 4, 204, 46, '2020-12-03 03:41:49', 0, 0, 5, 1, 5);
INSERT INTO `stock` VALUES (961, 36, 4, 204, 46, '2020-12-03 03:50:05', 0, 0, 6, 1, 5);
INSERT INTO `stock` VALUES (981, 36, 4, 200, 46, '2020-12-03 05:22:09', 0, 0, 8, 1, 5);
INSERT INTO `stock` VALUES (1065, 36, 4, 204, 46, '2020-12-03 14:57:47', 0, 0, 5, 1, 5);
INSERT INTO `stock` VALUES (1091, 37, 4, 200, 46, '2020-12-03 17:53:53', 1, 1, 8, 1, 5);
INSERT INTO `stock` VALUES (1121, 37, 4, 204, 47, '2020-12-03 20:24:10', 0, 0, 7, 1, 6);
INSERT INTO `stock` VALUES (1123, 35, 4, 204, 47, '2020-12-03 20:30:59', 1, 0, 3, 1, 6);
INSERT INTO `stock` VALUES (1150, 35, 4, 204, 47, '2020-12-03 22:40:29', 0, 0, 0, 1, 6);
INSERT INTO `stock` VALUES (1418, 39, 4, 204, 46, '2020-12-04 21:52:20', 0, 0, 5, 1, 5);
INSERT INTO `stock` VALUES (1462, 39, 4, 204, 46, '2020-12-05 01:28:55', 0, 0, 7, 1, 5);
INSERT INTO `stock` VALUES (1463, 38, 4, 204, 45, '2020-12-05 01:37:06', 0, 0, 1, 1, 5);
INSERT INTO `stock` VALUES (1622, 38, 4, 204, 47, '2020-12-05 13:47:04', 0, 0, 1, 1, 6);
INSERT INTO `stock` VALUES (1818, 40, 4, 204, 47, '2020-12-06 04:36:08', 1, 0, 2, 1, 6);
INSERT INTO `stock` VALUES (1899, 51, 4, 204, 47, '2020-12-06 12:00:41', 1, 0, 1, 1, 6);
INSERT INTO `stock` VALUES (1941, 51, 4, 204, 7, '2020-12-06 14:38:09', 0, 0, 4, 1, 1);
INSERT INTO `stock` VALUES (2041, 49, 4, 204, 46, '2020-12-06 22:01:23', 0, 0, 6, 1, 5);
INSERT INTO `stock` VALUES (2042, 50, 4, 204, 45, '2020-12-06 22:13:22', 0, 0, 7, 1, 5);
INSERT INTO `stock` VALUES (2296, 60, 4, 204, 45, '2020-12-07 16:57:50', 0, 0, 1, 1, 5);
INSERT INTO `stock` VALUES (2327, 61, 4, 200, 46, '2020-12-07 18:45:39', 1, 1, 8, 1, 5);
INSERT INTO `stock` VALUES (2365, 66, 4, 204, 46, '2020-12-07 20:51:18', 0, 0, 6, 1, 5);
INSERT INTO `stock` VALUES (2501, 72, 4, 204, 47, '2020-12-08 07:48:52', 1, 0, 1, 1, 6);
INSERT INTO `stock` VALUES (2502, 72, 4, 204, 47, '2020-12-08 08:04:52', 2, 0, 3, 1, 6);
INSERT INTO `stock` VALUES (2543, 76, 4, 204, 45, '2020-12-08 11:29:18', 0, 0, 1, 1, 5);
INSERT INTO `stock` VALUES (2693, 87, 4, 204, 45, '2020-12-09 01:23:56', 0, 0, 4, 1, 5);
INSERT INTO `stock` VALUES (2856, 100, 4, 200, 46, '2020-12-09 15:32:17', 0, 0, 8, 1, 5);
INSERT INTO `stock` VALUES (3233, 113, 4, 204, 46, '2020-12-10 17:53:17', 0, 0, 6, 1, 5);
INSERT INTO `stock` VALUES (5172, 180, 4, 204, 46, '2020-12-16 16:23:46', 0, 0, 6, 1, 5);
INSERT INTO `stock` VALUES (5189, 181, 4, 200, 46, '2020-12-16 18:34:43', 1, 1, 8, 1, 5);
INSERT INTO `stock` VALUES (5207, 197, 3, 162, 42, '2020-12-17 06:32:00', 0, 0, 8, 1, 4);
INSERT INTO `stock` VALUES (5209, 197, 3, 162, 42, '2020-12-17 06:32:00', 1, 1, 5, 1, 4);
INSERT INTO `stock` VALUES (5211, 197, 3, 162, 42, '2020-12-17 06:32:00', 2, 1, 6, 1, 4);
INSERT INTO `stock` VALUES (5212, 197, 3, 162, 40, '2020-12-17 06:32:00', 0, 0, 8, 1, 4);
INSERT INTO `stock` VALUES (5215, 197, 3, 162, 40, '2020-12-17 06:32:00', 1, 1, 6, 1, 4);
INSERT INTO `stock` VALUES (5218, 188, 3, 162, 49, '2020-12-17 03:10:00', 0, 0, 8, 1, 0);
INSERT INTO `stock` VALUES (5219, 188, 3, 162, 37, '2020-12-17 03:10:00', 0, 0, 7, 1, 4);
INSERT INTO `stock` VALUES (5222, 188, 3, 162, 50, '2020-12-17 03:10:00', 1, 0, 6, 1, 0);
INSERT INTO `stock` VALUES (5223, 188, 3, 162, 38, '2020-12-17 03:10:00', 2, 0, 5, 1, 4);
INSERT INTO `stock` VALUES (5225, 188, 3, 162, 37, '2020-12-17 03:10:00', 1, 1, 7, 1, 4);
INSERT INTO `stock` VALUES (5226, 188, 3, 162, 38, '2020-12-17 03:10:00', 3, 1, 8, 1, 4);
INSERT INTO `stock` VALUES (5228, 197, 3, 162, 42, '2020-12-17 06:32:00', 3, 1, 0, 1, 4);
INSERT INTO `stock` VALUES (5229, 188, 3, 162, 35, '2020-12-17 03:11:00', 0, 0, 8, 1, 4);
INSERT INTO `stock` VALUES (5230, 188, 3, 162, 37, '2020-12-17 03:10:00', 2, 1, 7, 1, 4);
INSERT INTO `stock` VALUES (5231, 188, 3, 162, 35, '2020-12-17 03:11:00', 1, 1, 6, 1, 4);
INSERT INTO `stock` VALUES (5232, 188, 3, 162, 33, '2020-12-17 03:12:15', 0, 0, 5, 1, 4);
INSERT INTO `stock` VALUES (5233, 188, 3, 162, 33, '2020-12-17 03:22:32', 1, 1, 5, 1, 4);
INSERT INTO `stock` VALUES (5240, 194, 3, 162, 31, '2020-12-17 06:18:00', 0, 0, 8, 1, 4);
INSERT INTO `stock` VALUES (5241, 194, 3, 162, 31, '2020-12-17 06:18:00', 1, 1, 8, 1, 4);
INSERT INTO `stock` VALUES (5243, 195, 3, 162, 47, '2020-12-17 06:18:00', 1, 0, 7, 1, 6);
INSERT INTO `stock` VALUES (5245, 192, 4, 204, 18, '2020-12-17 04:38:05', 2, 0, 3, 1, 0);
INSERT INTO `stock` VALUES (5247, 192, 4, 204, 24, '2020-12-17 04:42:12', 3, 0, 1, 1, 4);
INSERT INTO `stock` VALUES (5248, 194, 3, 162, 31, '2020-12-17 06:18:00', 2, 1, 8, 1, 4);
INSERT INTO `stock` VALUES (5249, 192, 4, 204, 24, '2020-12-17 04:49:21', 4, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5250, 194, 3, 162, 31, '2020-12-17 06:18:00', 3, 1, 5, 1, 4);
INSERT INTO `stock` VALUES (5251, 192, 4, 204, 22, '2020-12-17 05:05:39', 0, 0, 4, 1, 4);
INSERT INTO `stock` VALUES (5252, 192, 4, 204, 22, '2020-12-17 05:11:01', 1, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5254, 192, 4, 204, 22, '2020-12-17 05:14:50', 2, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5255, 192, 4, 204, 22, '2020-12-17 05:18:06', 3, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5256, 192, 4, 204, 22, '2020-12-17 05:35:46', 4, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5257, 193, 4, 204, 23, '2020-12-17 05:35:49', 0, 0, 2, 1, 4);
INSERT INTO `stock` VALUES (5258, 192, 4, 204, 28, '2020-12-17 05:43:59', 0, 0, 4, 1, 4);
INSERT INTO `stock` VALUES (5259, 194, 3, 162, 32, '2020-12-17 06:17:00', 1, 1, 8, 1, 4);
INSERT INTO `stock` VALUES (5260, 192, 4, 204, 28, '2020-12-17 05:45:38', 1, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5261, 194, 3, 162, 32, '2020-12-17 06:17:00', 0, 0, 0, 1, 4);
INSERT INTO `stock` VALUES (5262, 194, 3, 162, 32, '2020-12-17 06:17:00', 2, 1, 8, 1, 4);
INSERT INTO `stock` VALUES (5263, 192, 4, 204, 28, '2020-12-17 06:09:22', 2, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5264, 195, 3, 162, 36, '2020-12-17 06:18:00', 1, 0, 7, 1, 4);
INSERT INTO `stock` VALUES (5265, 192, 4, 204, 28, '2020-12-17 06:28:42', 3, 1, 4, 1, 4);
INSERT INTO `stock` VALUES (5266, 192, 4, 204, 28, '2020-12-17 06:32:37', 4, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5267, 194, 3, 162, 32, '2020-12-17 06:35:22', 3, 1, 8, 1, 4);
INSERT INTO `stock` VALUES (5268, 192, 4, 204, 27, '2020-12-17 06:35:59', 0, 0, 3, 1, 4);
INSERT INTO `stock` VALUES (5269, 194, 3, 162, 39, '2020-12-17 06:40:35', 1, 0, 5, 1, 4);
INSERT INTO `stock` VALUES (5270, 192, 4, 204, 27, '2020-12-17 06:40:59', 1, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5272, 192, 4, 204, 27, '2020-12-17 06:51:45', 2, 1, 4, 1, 4);
INSERT INTO `stock` VALUES (5273, 195, 3, 162, 36, '2020-12-17 06:53:57', 2, 1, 7, 1, 4);
INSERT INTO `stock` VALUES (5274, 192, 4, 204, 27, '2020-12-17 06:55:51', 3, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5275, 192, 4, 204, 27, '2020-12-17 06:59:34', 4, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5276, 192, 4, 204, 20, '2020-12-17 07:16:33', 0, 0, 1, 1, 4);
INSERT INTO `stock` VALUES (5277, 194, 3, 162, 39, '2020-12-17 07:16:33', 1, 1, 8, 1, 4);
INSERT INTO `stock` VALUES (5278, 194, 3, 162, 39, '2020-12-17 07:21:28', 2, 1, 6, 1, 4);
INSERT INTO `stock` VALUES (5279, 192, 4, 204, 20, '2020-12-17 07:26:58', 1, 1, 4, 1, 4);
INSERT INTO `stock` VALUES (5280, 195, 3, 162, 36, '2020-12-17 07:26:59', 3, 1, 7, 1, 4);
INSERT INTO `stock` VALUES (5281, 192, 4, 204, 20, '2020-12-17 07:32:30', 2, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5282, 192, 4, 204, 20, '2020-12-17 07:35:38', 3, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5283, 192, 4, 204, 20, '2020-12-17 07:41:39', 4, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5284, 194, 3, 162, 39, '2020-12-17 07:42:43', 3, 1, 8, 1, 4);
INSERT INTO `stock` VALUES (5285, 192, 4, 204, 19, '2020-12-17 07:54:47', 0, 0, 4, 1, 4);
INSERT INTO `stock` VALUES (5286, 192, 4, 204, 19, '2020-12-17 07:59:31', 1, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5289, 192, 4, 204, 19, '2020-12-17 08:04:27', 2, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5290, 185, 4, 204, 21, '2020-12-17 07:58:00', 0, 0, 0, 1, 4);
INSERT INTO `stock` VALUES (5291, 192, 4, 204, 19, '2020-12-17 08:07:39', 3, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5294, 196, 3, 162, 44, '2020-12-17 08:16:51', 1, 0, 8, 1, 4);
INSERT INTO `stock` VALUES (5295, 192, 4, 204, 19, '2020-12-17 08:20:48', 4, 1, 4, 1, 4);
INSERT INTO `stock` VALUES (5296, 192, 4, 204, 25, '2020-12-17 08:25:20', 0, 0, 1, 1, 4);
INSERT INTO `stock` VALUES (5297, 192, 4, 204, 25, '2020-12-17 08:29:13', 1, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5298, 192, 4, 204, 25, '2020-12-17 08:33:57', 2, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5300, 192, 4, 204, 25, '2020-12-17 08:51:14', 3, 1, 4, 1, 4);
INSERT INTO `stock` VALUES (5301, 192, 4, 204, 25, '2020-12-17 08:54:39', 4, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5302, 192, 4, 204, 26, '2020-12-17 08:58:04', 0, 0, 1, 1, 4);
INSERT INTO `stock` VALUES (5303, 192, 4, 204, 26, '2020-12-17 09:12:23', 1, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5304, 193, 4, 204, 23, '2020-12-17 09:20:36', 1, 1, 2, 1, 4);
INSERT INTO `stock` VALUES (5307, 192, 4, 204, 26, '2020-12-17 09:43:37', 2, 1, 1, 1, 4);
INSERT INTO `stock` VALUES (5308, 192, 4, 204, 26, '2020-12-17 09:47:04', 3, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5309, 192, 4, 204, 8, '2020-12-17 09:49:54', 0, 0, 4, 1, 1);
INSERT INTO `stock` VALUES (5310, 196, 3, 162, 44, '2020-12-17 09:50:00', 1, 1, 0, 1, 4);
INSERT INTO `stock` VALUES (5311, 192, 4, 204, 29, '2020-12-17 09:54:55', 0, 0, 1, 1, 4);
INSERT INTO `stock` VALUES (5317, 192, 4, 204, 29, '2020-12-17 10:02:47', 1, 1, 4, 1, 4);
INSERT INTO `stock` VALUES (5318, 192, 4, 204, 29, '2020-12-17 10:06:13', 2, 1, 3, 1, 4);
INSERT INTO `stock` VALUES (5319, 194, 3, 162, 15, '2020-12-17 10:11:26', 0, 0, 8, 1, 1);
INSERT INTO `stock` VALUES (5320, 192, 4, 204, 1, '2020-12-17 10:11:46', 0, 0, 1, 1, 1);
INSERT INTO `stock` VALUES (5321, 33, 4, 204, 19, '2020-12-31 16:59:38', 5, 1, 0, 1, 4);
INSERT INTO `stock` VALUES (5322, 49, 4, 204, 19, '2020-12-31 16:59:48', 6, 1, 0, 1, 4);

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
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10420 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '库存出入记录' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_log
-- ----------------------------
INSERT INTO `stock_log` VALUES (9668, 159, 4, 204, 18, 9, '2020-12-16 00:00:26');
INSERT INTO `stock_log` VALUES (9669, 163, 4, 204, 9, 5, '2020-12-16 00:02:50');
INSERT INTO `stock_log` VALUES (9670, 161, 4, 204, 50, 10, '2020-12-16 00:05:05');
INSERT INTO `stock_log` VALUES (9671, 163, 4, 204, 11, 6, '2020-12-16 00:07:47');
INSERT INTO `stock_log` VALUES (9672, 174, 4, 204, 2, 1, '2020-12-16 00:10:26');
INSERT INTO `stock_log` VALUES (9673, 161, 4, 204, 49, 10, '2020-12-16 00:16:15');
INSERT INTO `stock_log` VALUES (9674, 174, 4, 204, 7, 4, '2020-12-16 00:16:16');
INSERT INTO `stock_log` VALUES (9675, 174, 4, 204, 6, 3, '2020-12-16 00:20:43');
INSERT INTO `stock_log` VALUES (9676, 159, 4, 204, 17, 9, '2020-12-16 00:24:02');
INSERT INTO `stock_log` VALUES (9677, 174, 4, 204, 1, 1, '2020-12-16 00:25:22');
INSERT INTO `stock_log` VALUES (9678, 161, 4, 204, 50, 10, '2020-12-16 00:29:16');
INSERT INTO `stock_log` VALUES (9679, 174, 4, 204, 2, 1, '2020-12-16 00:37:16');
INSERT INTO `stock_log` VALUES (9680, 161, 4, 204, 49, 10, '2020-12-16 00:40:01');
INSERT INTO `stock_log` VALUES (9681, 174, 4, 204, 8, 4, '2020-12-16 00:41:49');
INSERT INTO `stock_log` VALUES (9682, 159, 4, 204, 18, 9, '2020-12-16 00:42:05');
INSERT INTO `stock_log` VALUES (9683, 163, 4, 200, 15, 8, '2020-12-16 00:46:06');
INSERT INTO `stock_log` VALUES (9684, 174, 4, 204, 5, 3, '2020-12-16 00:47:05');
INSERT INTO `stock_log` VALUES (9685, 163, 4, 204, 10, 5, '2020-12-16 00:49:59');
INSERT INTO `stock_log` VALUES (9686, 163, 4, 200, 50, 10, '2020-12-16 00:51:48');
INSERT INTO `stock_log` VALUES (9687, 174, 4, 204, 1, 1, '2020-12-16 00:52:42');
INSERT INTO `stock_log` VALUES (9688, 159, 4, 204, 17, 9, '2020-12-16 00:52:53');
INSERT INTO `stock_log` VALUES (9689, 163, 4, 204, 12, 6, '2020-12-16 00:55:43');
INSERT INTO `stock_log` VALUES (9690, 163, 4, 200, 49, 10, '2020-12-16 00:57:17');
INSERT INTO `stock_log` VALUES (9691, 163, 4, 200, 16, 8, '2020-12-16 01:00:47');
INSERT INTO `stock_log` VALUES (9692, 174, 4, 204, 2, 1, '2020-12-16 01:08:29');
INSERT INTO `stock_log` VALUES (9693, 163, 4, 200, 15, 8, '2020-12-16 01:09:14');
INSERT INTO `stock_log` VALUES (9694, 163, 4, 204, 50, 10, '2020-12-16 01:09:55');
INSERT INTO `stock_log` VALUES (9695, 159, 4, 204, 18, 9, '2020-12-16 01:11:17');
INSERT INTO `stock_log` VALUES (9696, 174, 4, 204, 6, 3, '2020-12-16 01:12:13');
INSERT INTO `stock_log` VALUES (9697, 163, 4, 204, 9, 5, '2020-12-16 01:13:55');
INSERT INTO `stock_log` VALUES (9698, 174, 4, 204, 7, 4, '2020-12-16 01:17:42');
INSERT INTO `stock_log` VALUES (9699, 163, 4, 204, 11, 6, '2020-12-16 01:17:55');
INSERT INTO `stock_log` VALUES (9700, 163, 4, 204, 49, 10, '2020-12-16 01:20:26');
INSERT INTO `stock_log` VALUES (9701, 159, 4, 204, 17, 9, '2020-12-16 01:21:20');
INSERT INTO `stock_log` VALUES (9702, 163, 4, 200, 16, 8, '2020-12-16 01:22:28');
INSERT INTO `stock_log` VALUES (9703, 174, 4, 204, 1, 1, '2020-12-16 01:22:58');
INSERT INTO `stock_log` VALUES (9704, 163, 4, 200, 50, 10, '2020-12-16 01:31:28');
INSERT INTO `stock_log` VALUES (9705, 163, 4, 200, 15, 8, '2020-12-16 01:33:17');
INSERT INTO `stock_log` VALUES (9706, 174, 4, 204, 8, 4, '2020-12-16 01:34:28');
INSERT INTO `stock_log` VALUES (9707, 174, 4, 204, 18, 9, '2020-12-16 01:37:50');
INSERT INTO `stock_log` VALUES (9708, 163, 4, 204, 10, 5, '2020-12-16 01:37:55');
INSERT INTO `stock_log` VALUES (9709, 174, 4, 204, 2, 1, '2020-12-16 01:39:33');
INSERT INTO `stock_log` VALUES (9710, 163, 4, 200, 49, 10, '2020-12-16 01:41:04');
INSERT INTO `stock_log` VALUES (9711, 174, 4, 204, 17, 9, '2020-12-16 01:42:30');
INSERT INTO `stock_log` VALUES (9712, 163, 4, 204, 12, 6, '2020-12-16 01:42:52');
INSERT INTO `stock_log` VALUES (9713, 174, 4, 204, 5, 3, '2020-12-16 01:43:09');
INSERT INTO `stock_log` VALUES (9714, 163, 4, 200, 16, 8, '2020-12-16 01:47:03');
INSERT INTO `stock_log` VALUES (9715, 174, 4, 204, 1, 1, '2020-12-16 01:48:07');
INSERT INTO `stock_log` VALUES (9716, 163, 4, 204, 50, 10, '2020-12-16 01:51:06');
INSERT INTO `stock_log` VALUES (9717, 175, 4, 204, 3, 2, '2020-12-16 01:52:52');
INSERT INTO `stock_log` VALUES (9718, 163, 4, 200, 15, 8, '2020-12-16 01:56:19');
INSERT INTO `stock_log` VALUES (9719, 174, 4, 204, 18, 9, '2020-12-16 01:58:42');
INSERT INTO `stock_log` VALUES (9720, 163, 4, 204, 49, 10, '2020-12-16 02:00:19');
INSERT INTO `stock_log` VALUES (9721, 174, 4, 204, 7, 4, '2020-12-16 02:05:59');
INSERT INTO `stock_log` VALUES (9722, 163, 4, 204, 11, 6, '2020-12-16 02:08:46');
INSERT INTO `stock_log` VALUES (9723, 163, 4, 200, 50, 10, '2020-12-16 02:08:50');
INSERT INTO `stock_log` VALUES (9724, 174, 4, 204, 17, 9, '2020-12-16 02:08:57');
INSERT INTO `stock_log` VALUES (9725, 174, 4, 204, 2, 1, '2020-12-16 02:10:17');
INSERT INTO `stock_log` VALUES (9726, 174, 4, 204, 6, 3, '2020-12-16 02:14:07');
INSERT INTO `stock_log` VALUES (9727, 163, 4, 204, 9, 5, '2020-12-16 02:16:18');
INSERT INTO `stock_log` VALUES (9728, 174, 4, 204, 1, 1, '2020-12-16 02:19:23');
INSERT INTO `stock_log` VALUES (9729, 163, 4, 200, 49, 10, '2020-12-16 02:19:43');
INSERT INTO `stock_log` VALUES (9730, 163, 4, 200, 16, 8, '2020-12-16 02:20:27');
INSERT INTO `stock_log` VALUES (9731, 174, 4, 204, 18, 9, '2020-12-16 02:20:52');
INSERT INTO `stock_log` VALUES (9732, 164, 4, 204, 14, 7, '2020-12-16 02:23:14');
INSERT INTO `stock_log` VALUES (9733, 163, 4, 204, 50, 10, '2020-12-16 02:29:11');
INSERT INTO `stock_log` VALUES (9734, 174, 4, 204, 17, 9, '2020-12-16 02:30:37');
INSERT INTO `stock_log` VALUES (9735, 174, 4, 204, 8, 4, '2020-12-16 02:30:43');
INSERT INTO `stock_log` VALUES (9736, 163, 4, 204, 12, 6, '2020-12-16 02:31:25');
INSERT INTO `stock_log` VALUES (9737, 174, 4, 204, 5, 3, '2020-12-16 02:35:53');
INSERT INTO `stock_log` VALUES (9738, 163, 4, 204, 10, 5, '2020-12-16 02:36:41');
INSERT INTO `stock_log` VALUES (9739, 174, 4, 204, 2, 1, '2020-12-16 02:39:31');
INSERT INTO `stock_log` VALUES (9740, 163, 4, 204, 49, 10, '2020-12-16 02:39:37');
INSERT INTO `stock_log` VALUES (9741, 174, 4, 204, 18, 9, '2020-12-16 02:40:59');
INSERT INTO `stock_log` VALUES (9742, 163, 4, 200, 15, 8, '2020-12-16 02:42:21');
INSERT INTO `stock_log` VALUES (9743, 174, 4, 204, 1, 1, '2020-12-16 02:43:37');
INSERT INTO `stock_log` VALUES (9744, 163, 4, 200, 16, 8, '2020-12-16 02:45:55');
INSERT INTO `stock_log` VALUES (9745, 163, 4, 200, 50, 10, '2020-12-16 02:50:32');
INSERT INTO `stock_log` VALUES (9746, 174, 4, 204, 7, 4, '2020-12-16 02:53:44');
INSERT INTO `stock_log` VALUES (9747, 174, 4, 204, 17, 9, '2020-12-16 02:56:19');
INSERT INTO `stock_log` VALUES (9748, 174, 4, 204, 6, 3, '2020-12-16 02:58:27');
INSERT INTO `stock_log` VALUES (9749, 163, 4, 200, 49, 10, '2020-12-16 02:59:17');
INSERT INTO `stock_log` VALUES (9750, 163, 4, 200, 15, 8, '2020-12-16 02:59:48');
INSERT INTO `stock_log` VALUES (9751, 174, 4, 204, 2, 1, '2020-12-16 03:03:41');
INSERT INTO `stock_log` VALUES (9752, 163, 4, 204, 9, 5, '2020-12-16 03:03:52');
INSERT INTO `stock_log` VALUES (9753, 174, 4, 204, 18, 9, '2020-12-16 03:04:04');
INSERT INTO `stock_log` VALUES (9754, 174, 4, 204, 1, 1, '2020-12-16 03:08:06');
INSERT INTO `stock_log` VALUES (9755, 163, 4, 204, 11, 6, '2020-12-16 03:08:44');
INSERT INTO `stock_log` VALUES (9756, 163, 4, 204, 50, 10, '2020-12-16 03:11:50');
INSERT INTO `stock_log` VALUES (9757, 163, 4, 200, 16, 8, '2020-12-16 03:12:46');
INSERT INTO `stock_log` VALUES (9758, 174, 4, 204, 17, 9, '2020-12-16 03:14:14');
INSERT INTO `stock_log` VALUES (9759, 174, 4, 204, 8, 4, '2020-12-16 03:18:23');
INSERT INTO `stock_log` VALUES (9760, 163, 4, 204, 49, 10, '2020-12-16 03:21:23');
INSERT INTO `stock_log` VALUES (9761, 174, 4, 204, 5, 3, '2020-12-16 03:23:15');
INSERT INTO `stock_log` VALUES (9762, 163, 4, 200, 15, 8, '2020-12-16 03:23:20');
INSERT INTO `stock_log` VALUES (9763, 163, 4, 204, 10, 5, '2020-12-16 03:27:09');
INSERT INTO `stock_log` VALUES (9764, 174, 4, 204, 18, 9, '2020-12-16 03:27:38');
INSERT INTO `stock_log` VALUES (9765, 174, 4, 204, 2, 1, '2020-12-16 03:27:48');
INSERT INTO `stock_log` VALUES (9766, 174, 4, 204, 1, 1, '2020-12-16 03:32:22');
INSERT INTO `stock_log` VALUES (9767, 163, 4, 200, 50, 10, '2020-12-16 03:32:57');
INSERT INTO `stock_log` VALUES (9768, 163, 4, 204, 12, 6, '2020-12-16 03:34:40');
INSERT INTO `stock_log` VALUES (9769, 174, 4, 204, 17, 9, '2020-12-16 03:35:44');
INSERT INTO `stock_log` VALUES (9770, 163, 4, 200, 16, 8, '2020-12-16 03:39:56');
INSERT INTO `stock_log` VALUES (9771, 174, 4, 204, 7, 4, '2020-12-16 03:42:52');
INSERT INTO `stock_log` VALUES (9772, 163, 4, 200, 49, 10, '2020-12-16 03:44:11');
INSERT INTO `stock_log` VALUES (9773, 163, 4, 200, 15, 8, '2020-12-16 03:45:11');
INSERT INTO `stock_log` VALUES (9774, 174, 4, 204, 18, 9, '2020-12-16 03:45:24');
INSERT INTO `stock_log` VALUES (9775, 174, 4, 204, 6, 3, '2020-12-16 03:47:12');
INSERT INTO `stock_log` VALUES (9776, 163, 4, 204, 9, 5, '2020-12-16 03:48:48');
INSERT INTO `stock_log` VALUES (9777, 174, 4, 204, 2, 1, '2020-12-16 03:51:37');
INSERT INTO `stock_log` VALUES (9778, 175, 4, 204, 4, 2, '2020-12-16 03:52:55');
INSERT INTO `stock_log` VALUES (9779, 163, 4, 204, 11, 6, '2020-12-16 03:53:15');
INSERT INTO `stock_log` VALUES (9780, 174, 4, 204, 17, 9, '2020-12-16 03:54:58');
INSERT INTO `stock_log` VALUES (9781, 174, 4, 204, 1, 1, '2020-12-16 03:56:25');
INSERT INTO `stock_log` VALUES (9782, 163, 4, 200, 16, 8, '2020-12-16 03:56:50');
INSERT INTO `stock_log` VALUES (9783, 163, 4, 204, 50, 10, '2020-12-16 03:57:26');
INSERT INTO `stock_log` VALUES (9784, 174, 4, 204, 18, 9, '2020-12-16 04:04:41');
INSERT INTO `stock_log` VALUES (9785, 174, 4, 204, 8, 4, '2020-12-16 04:06:20');
INSERT INTO `stock_log` VALUES (9786, 163, 4, 204, 49, 10, '2020-12-16 04:07:26');
INSERT INTO `stock_log` VALUES (9787, 174, 4, 204, 2, 1, '2020-12-16 04:11:04');
INSERT INTO `stock_log` VALUES (9788, 174, 4, 204, 17, 9, '2020-12-16 04:13:07');
INSERT INTO `stock_log` VALUES (9789, 174, 4, 204, 5, 3, '2020-12-16 04:15:15');
INSERT INTO `stock_log` VALUES (9790, 163, 4, 200, 15, 8, '2020-12-16 04:15:18');
INSERT INTO `stock_log` VALUES (9791, 163, 4, 200, 50, 10, '2020-12-16 04:17:10');
INSERT INTO `stock_log` VALUES (9792, 174, 4, 204, 1, 1, '2020-12-16 04:18:52');
INSERT INTO `stock_log` VALUES (9793, 163, 4, 204, 12, 6, '2020-12-16 04:19:23');
INSERT INTO `stock_log` VALUES (9794, 163, 4, 204, 10, 5, '2020-12-16 04:23:30');
INSERT INTO `stock_log` VALUES (9795, 174, 4, 204, 18, 9, '2020-12-16 04:23:35');
INSERT INTO `stock_log` VALUES (9796, 163, 4, 200, 49, 10, '2020-12-16 04:28:34');
INSERT INTO `stock_log` VALUES (9797, 174, 4, 204, 7, 4, '2020-12-16 04:31:01');
INSERT INTO `stock_log` VALUES (9798, 174, 4, 204, 17, 9, '2020-12-16 04:32:43');
INSERT INTO `stock_log` VALUES (9799, 163, 4, 200, 16, 8, '2020-12-16 04:33:01');
INSERT INTO `stock_log` VALUES (9800, 174, 4, 204, 2, 1, '2020-12-16 04:35:42');
INSERT INTO `stock_log` VALUES (9801, 174, 4, 204, 6, 3, '2020-12-16 04:39:49');
INSERT INTO `stock_log` VALUES (9802, 174, 4, 204, 18, 9, '2020-12-16 04:43:04');
INSERT INTO `stock_log` VALUES (9803, 174, 4, 204, 1, 1, '2020-12-16 04:44:23');
INSERT INTO `stock_log` VALUES (9804, 163, 4, 200, 15, 8, '2020-12-16 04:44:39');
INSERT INTO `stock_log` VALUES (9805, 163, 4, 204, 9, 5, '2020-12-16 04:48:34');
INSERT INTO `stock_log` VALUES (9806, 163, 4, 204, 50, 10, '2020-12-16 04:50:14');
INSERT INTO `stock_log` VALUES (9807, 163, 4, 204, 11, 6, '2020-12-16 04:53:08');
INSERT INTO `stock_log` VALUES (9808, 174, 4, 204, 17, 9, '2020-12-16 04:54:09');
INSERT INTO `stock_log` VALUES (9809, 163, 4, 204, 49, 10, '2020-12-16 04:58:59');
INSERT INTO `stock_log` VALUES (9810, 174, 4, 204, 18, 9, '2020-12-16 05:05:07');
INSERT INTO `stock_log` VALUES (9811, 163, 4, 200, 16, 8, '2020-12-16 05:05:51');
INSERT INTO `stock_log` VALUES (9812, 163, 4, 200, 50, 10, '2020-12-16 05:09:02');
INSERT INTO `stock_log` VALUES (9813, 174, 4, 204, 17, 9, '2020-12-16 05:17:27');
INSERT INTO `stock_log` VALUES (9814, 163, 4, 204, 49, 10, '2020-12-16 05:18:52');
INSERT INTO `stock_log` VALUES (9815, 174, 4, 204, 8, 4, '2020-12-16 05:19:22');
INSERT INTO `stock_log` VALUES (9816, 174, 4, 204, 5, 3, '2020-12-16 05:24:57');
INSERT INTO `stock_log` VALUES (9817, 163, 4, 200, 15, 8, '2020-12-16 05:27:23');
INSERT INTO `stock_log` VALUES (9818, 174, 4, 204, 18, 9, '2020-12-16 05:27:58');
INSERT INTO `stock_log` VALUES (9819, 164, 4, 204, 13, 7, '2020-12-16 05:28:50');
INSERT INTO `stock_log` VALUES (9820, 174, 4, 204, 2, 1, '2020-12-16 05:28:59');
INSERT INTO `stock_log` VALUES (9821, 163, 4, 204, 50, 10, '2020-12-16 05:29:23');
INSERT INTO `stock_log` VALUES (9822, 163, 4, 204, 10, 5, '2020-12-16 05:31:33');
INSERT INTO `stock_log` VALUES (9823, 174, 4, 204, 1, 1, '2020-12-16 05:35:15');
INSERT INTO `stock_log` VALUES (9824, 174, 4, 204, 17, 9, '2020-12-16 05:35:36');
INSERT INTO `stock_log` VALUES (9825, 163, 4, 200, 49, 10, '2020-12-16 05:39:34');
INSERT INTO `stock_log` VALUES (9826, 163, 4, 200, 16, 8, '2020-12-16 05:39:38');
INSERT INTO `stock_log` VALUES (9827, 163, 4, 204, 12, 6, '2020-12-16 05:44:18');
INSERT INTO `stock_log` VALUES (9828, 174, 4, 204, 7, 4, '2020-12-16 05:44:38');
INSERT INTO `stock_log` VALUES (9829, 174, 4, 204, 18, 9, '2020-12-16 05:47:46');
INSERT INTO `stock_log` VALUES (9830, 174, 4, 204, 6, 3, '2020-12-16 05:48:24');
INSERT INTO `stock_log` VALUES (9831, 163, 4, 200, 50, 10, '2020-12-16 05:50:26');
INSERT INTO `stock_log` VALUES (9832, 174, 4, 204, 2, 1, '2020-12-16 05:53:19');
INSERT INTO `stock_log` VALUES (9833, 175, 4, 204, 3, 2, '2020-12-16 05:54:10');
INSERT INTO `stock_log` VALUES (9834, 174, 4, 204, 17, 9, '2020-12-16 05:57:17');
INSERT INTO `stock_log` VALUES (9835, 163, 4, 200, 15, 8, '2020-12-16 05:59:18');
INSERT INTO `stock_log` VALUES (9836, 174, 4, 204, 1, 1, '2020-12-16 05:59:32');
INSERT INTO `stock_log` VALUES (9837, 163, 4, 200, 49, 10, '2020-12-16 05:59:45');
INSERT INTO `stock_log` VALUES (9838, 163, 4, 204, 11, 6, '2020-12-16 06:02:50');
INSERT INTO `stock_log` VALUES (9839, 163, 4, 204, 9, 5, '2020-12-16 06:06:11');
INSERT INTO `stock_log` VALUES (9840, 174, 4, 204, 18, 9, '2020-12-16 06:08:12');
INSERT INTO `stock_log` VALUES (9841, 174, 4, 204, 8, 4, '2020-12-16 06:08:53');
INSERT INTO `stock_log` VALUES (9842, 163, 4, 204, 50, 10, '2020-12-16 06:11:54');
INSERT INTO `stock_log` VALUES (9843, 174, 4, 204, 2, 1, '2020-12-16 06:13:20');
INSERT INTO `stock_log` VALUES (9844, 174, 4, 204, 17, 9, '2020-12-16 06:16:21');
INSERT INTO `stock_log` VALUES (9845, 174, 4, 204, 5, 3, '2020-12-16 06:17:11');
INSERT INTO `stock_log` VALUES (9846, 174, 4, 204, 1, 1, '2020-12-16 06:21:45');
INSERT INTO `stock_log` VALUES (9847, 163, 4, 204, 49, 10, '2020-12-16 06:22:17');
INSERT INTO `stock_log` VALUES (9848, 163, 4, 200, 16, 8, '2020-12-16 06:24:49');
INSERT INTO `stock_log` VALUES (9849, 174, 4, 204, 7, 4, '2020-12-16 06:30:59');
INSERT INTO `stock_log` VALUES (9850, 174, 4, 204, 18, 9, '2020-12-16 06:32:09');
INSERT INTO `stock_log` VALUES (9851, 163, 4, 204, 10, 5, '2020-12-16 06:34:21');
INSERT INTO `stock_log` VALUES (9852, 163, 4, 200, 50, 10, '2020-12-16 06:34:23');
INSERT INTO `stock_log` VALUES (9853, 174, 4, 204, 6, 3, '2020-12-16 06:35:01');
INSERT INTO `stock_log` VALUES (9854, 174, 4, 204, 2, 1, '2020-12-16 06:39:25');
INSERT INTO `stock_log` VALUES (9855, 163, 4, 200, 15, 8, '2020-12-16 06:39:43');
INSERT INTO `stock_log` VALUES (9856, 174, 4, 204, 17, 9, '2020-12-16 06:40:45');
INSERT INTO `stock_log` VALUES (9857, 163, 4, 204, 12, 6, '2020-12-16 06:43:02');
INSERT INTO `stock_log` VALUES (9858, 174, 4, 204, 1, 1, '2020-12-16 06:43:41');
INSERT INTO `stock_log` VALUES (9859, 163, 4, 200, 49, 10, '2020-12-16 06:44:16');
INSERT INTO `stock_log` VALUES (9860, 174, 4, 204, 18, 9, '2020-12-16 06:48:41');
INSERT INTO `stock_log` VALUES (9861, 163, 4, 200, 16, 8, '2020-12-16 06:50:43');
INSERT INTO `stock_log` VALUES (9862, 163, 4, 204, 50, 10, '2020-12-16 06:54:18');
INSERT INTO `stock_log` VALUES (9863, 174, 4, 204, 8, 4, '2020-12-16 06:54:41');
INSERT INTO `stock_log` VALUES (9864, 174, 4, 204, 5, 3, '2020-12-16 06:59:24');
INSERT INTO `stock_log` VALUES (9865, 174, 4, 204, 2, 1, '2020-12-16 07:02:30');
INSERT INTO `stock_log` VALUES (9866, 174, 4, 204, 17, 9, '2020-12-16 07:02:38');
INSERT INTO `stock_log` VALUES (9867, 163, 4, 204, 49, 10, '2020-12-16 07:06:21');
INSERT INTO `stock_log` VALUES (9868, 174, 4, 204, 1, 1, '2020-12-16 07:07:21');
INSERT INTO `stock_log` VALUES (9869, 174, 4, 204, 18, 9, '2020-12-16 07:16:36');
INSERT INTO `stock_log` VALUES (9870, 163, 4, 200, 50, 10, '2020-12-16 07:16:42');
INSERT INTO `stock_log` VALUES (9871, 174, 4, 204, 7, 4, '2020-12-16 07:18:01');
INSERT INTO `stock_log` VALUES (9872, 174, 4, 204, 6, 3, '2020-12-16 07:22:57');
INSERT INTO `stock_log` VALUES (9873, 174, 4, 204, 17, 9, '2020-12-16 07:24:59');
INSERT INTO `stock_log` VALUES (9874, 163, 4, 200, 49, 10, '2020-12-16 07:26:21');
INSERT INTO `stock_log` VALUES (9875, 174, 4, 204, 2, 1, '2020-12-16 07:28:13');
INSERT INTO `stock_log` VALUES (9876, 174, 4, 204, 1, 1, '2020-12-16 07:32:33');
INSERT INTO `stock_log` VALUES (9877, 163, 4, 204, 50, 10, '2020-12-16 07:34:10');
INSERT INTO `stock_log` VALUES (9878, 163, 4, 204, 9, 5, '2020-12-16 07:34:18');
INSERT INTO `stock_log` VALUES (9879, 174, 4, 204, 18, 9, '2020-12-16 07:36:47');
INSERT INTO `stock_log` VALUES (9880, 163, 4, 200, 15, 8, '2020-12-16 07:38:16');
INSERT INTO `stock_log` VALUES (9881, 174, 4, 204, 8, 4, '2020-12-16 07:42:35');
INSERT INTO `stock_log` VALUES (9882, 163, 4, 204, 49, 10, '2020-12-16 07:43:06');
INSERT INTO `stock_log` VALUES (9883, 163, 4, 204, 11, 6, '2020-12-16 07:43:11');
INSERT INTO `stock_log` VALUES (9884, 174, 4, 204, 17, 9, '2020-12-16 07:44:27');
INSERT INTO `stock_log` VALUES (9885, 174, 4, 204, 2, 1, '2020-12-16 07:47:28');
INSERT INTO `stock_log` VALUES (9886, 163, 4, 200, 50, 10, '2020-12-16 07:51:10');
INSERT INTO `stock_log` VALUES (9887, 174, 4, 204, 5, 3, '2020-12-16 07:52:15');
INSERT INTO `stock_log` VALUES (9888, 174, 4, 204, 18, 9, '2020-12-16 07:52:54');
INSERT INTO `stock_log` VALUES (9889, 174, 4, 204, 17, 9, '2020-12-16 08:01:16');
INSERT INTO `stock_log` VALUES (9890, 163, 4, 200, 49, 10, '2020-12-16 08:01:17');
INSERT INTO `stock_log` VALUES (9891, 174, 4, 204, 1, 1, '2020-12-16 08:02:54');
INSERT INTO `stock_log` VALUES (9892, 163, 4, 200, 16, 8, '2020-12-16 08:03:20');
INSERT INTO `stock_log` VALUES (9893, 163, 4, 204, 50, 10, '2020-12-16 08:09:03');
INSERT INTO `stock_log` VALUES (9894, 174, 4, 204, 18, 9, '2020-12-16 08:09:14');
INSERT INTO `stock_log` VALUES (9895, 163, 4, 204, 49, 10, '2020-12-16 08:16:46');
INSERT INTO `stock_log` VALUES (9896, 174, 4, 204, 17, 9, '2020-12-16 08:16:56');
INSERT INTO `stock_log` VALUES (9897, 174, 4, 204, 17, 9, '2020-12-16 08:18:14');
INSERT INTO `stock_log` VALUES (9898, 175, 4, 204, 4, 2, '2020-12-16 08:20:21');
INSERT INTO `stock_log` VALUES (9899, 174, 4, 204, 7, 4, '2020-12-16 08:20:48');
INSERT INTO `stock_log` VALUES (9900, 163, 4, 204, 10, 5, '2020-12-16 08:25:27');
INSERT INTO `stock_log` VALUES (9901, 174, 4, 204, 18, 9, '2020-12-16 08:25:32');
INSERT INTO `stock_log` VALUES (9902, 174, 4, 204, 2, 1, '2020-12-16 08:27:58');
INSERT INTO `stock_log` VALUES (9903, 163, 4, 200, 50, 10, '2020-12-16 08:28:31');
INSERT INTO `stock_log` VALUES (9904, 163, 4, 200, 15, 8, '2020-12-16 08:30:46');
INSERT INTO `stock_log` VALUES (9905, 174, 4, 204, 6, 3, '2020-12-16 08:31:39');
INSERT INTO `stock_log` VALUES (9906, 174, 4, 204, 17, 9, '2020-12-16 08:33:44');
INSERT INTO `stock_log` VALUES (9907, 163, 4, 204, 12, 6, '2020-12-16 08:34:20');
INSERT INTO `stock_log` VALUES (9908, 163, 4, 200, 16, 8, '2020-12-16 08:39:03');
INSERT INTO `stock_log` VALUES (9909, 163, 4, 200, 49, 10, '2020-12-16 08:40:46');
INSERT INTO `stock_log` VALUES (9910, 174, 4, 204, 18, 9, '2020-12-16 08:43:02');
INSERT INTO `stock_log` VALUES (9911, 174, 4, 204, 1, 1, '2020-12-16 08:48:23');
INSERT INTO `stock_log` VALUES (9912, 163, 4, 204, 50, 10, '2020-12-16 08:49:14');
INSERT INTO `stock_log` VALUES (9913, 163, 4, 204, 11, 6, '2020-12-16 08:49:33');
INSERT INTO `stock_log` VALUES (9914, 164, 4, 204, 14, 7, '2020-12-16 08:50:36');
INSERT INTO `stock_log` VALUES (9915, 174, 4, 204, 17, 9, '2020-12-16 08:51:11');
INSERT INTO `stock_log` VALUES (9916, 163, 4, 200, 15, 8, '2020-12-16 08:53:08');
INSERT INTO `stock_log` VALUES (9917, 163, 4, 204, 49, 10, '2020-12-16 08:59:07');
INSERT INTO `stock_log` VALUES (9918, 174, 4, 204, 18, 9, '2020-12-16 08:59:33');
INSERT INTO `stock_log` VALUES (9919, 174, 4, 204, 8, 4, '2020-12-16 08:59:59');
INSERT INTO `stock_log` VALUES (9920, 163, 4, 204, 9, 5, '2020-12-16 09:02:33');
INSERT INTO `stock_log` VALUES (9921, 174, 4, 204, 5, 3, '2020-12-16 09:05:08');
INSERT INTO `stock_log` VALUES (9922, 163, 4, 200, 50, 10, '2020-12-16 09:08:20');
INSERT INTO `stock_log` VALUES (9923, 174, 4, 204, 2, 1, '2020-12-16 09:08:24');
INSERT INTO `stock_log` VALUES (9924, 174, 4, 204, 17, 9, '2020-12-16 09:08:34');
INSERT INTO `stock_log` VALUES (9925, 174, 4, 204, 1, 1, '2020-12-16 09:15:28');
INSERT INTO `stock_log` VALUES (9926, 174, 4, 204, 18, 9, '2020-12-16 09:16:08');
INSERT INTO `stock_log` VALUES (9927, 163, 4, 200, 49, 10, '2020-12-16 09:16:37');
INSERT INTO `stock_log` VALUES (9928, 176, 4, 200, 16, 8, '2020-12-16 09:19:48');
INSERT INTO `stock_log` VALUES (9929, 174, 4, 204, 17, 9, '2020-12-16 09:25:07');
INSERT INTO `stock_log` VALUES (9930, 174, 4, 204, 7, 4, '2020-12-16 09:25:58');
INSERT INTO `stock_log` VALUES (9931, 163, 4, 204, 50, 10, '2020-12-16 09:28:33');
INSERT INTO `stock_log` VALUES (9932, 176, 4, 200, 15, 8, '2020-12-16 09:30:16');
INSERT INTO `stock_log` VALUES (9933, 174, 4, 204, 6, 3, '2020-12-16 09:30:31');
INSERT INTO `stock_log` VALUES (9934, 176, 4, 204, 12, 6, '2020-12-16 09:34:23');
INSERT INTO `stock_log` VALUES (9935, 174, 4, 204, 18, 9, '2020-12-16 09:34:40');
INSERT INTO `stock_log` VALUES (9936, 174, 4, 204, 2, 1, '2020-12-16 09:35:23');
INSERT INTO `stock_log` VALUES (9937, 163, 4, 200, 49, 10, '2020-12-16 09:36:27');
INSERT INTO `stock_log` VALUES (9938, 176, 4, 204, 10, 5, '2020-12-16 09:38:32');
INSERT INTO `stock_log` VALUES (9939, 174, 4, 204, 1, 1, '2020-12-16 09:39:20');
INSERT INTO `stock_log` VALUES (9940, 176, 4, 200, 16, 8, '2020-12-16 09:42:48');
INSERT INTO `stock_log` VALUES (9941, 174, 4, 204, 17, 9, '2020-12-16 09:43:18');
INSERT INTO `stock_log` VALUES (9942, 163, 4, 204, 50, 10, '2020-12-16 09:45:15');
INSERT INTO `stock_log` VALUES (9943, 174, 4, 204, 8, 4, '2020-12-16 09:48:32');
INSERT INTO `stock_log` VALUES (9944, 174, 4, 204, 18, 9, '2020-12-16 09:51:19');
INSERT INTO `stock_log` VALUES (9945, 176, 4, 200, 15, 8, '2020-12-16 09:52:10');
INSERT INTO `stock_log` VALUES (9946, 163, 4, 200, 49, 10, '2020-12-16 09:52:48');
INSERT INTO `stock_log` VALUES (9947, 174, 4, 204, 2, 1, '2020-12-16 09:54:00');
INSERT INTO `stock_log` VALUES (9948, 176, 4, 204, 9, 5, '2020-12-16 09:56:20');
INSERT INTO `stock_log` VALUES (9949, 174, 4, 204, 5, 3, '2020-12-16 09:58:13');
INSERT INTO `stock_log` VALUES (9950, 176, 4, 204, 11, 6, '2020-12-16 10:00:49');
INSERT INTO `stock_log` VALUES (9951, 174, 4, 204, 17, 9, '2020-12-16 10:00:56');
INSERT INTO `stock_log` VALUES (9952, 163, 4, 204, 50, 10, '2020-12-16 10:01:52');
INSERT INTO `stock_log` VALUES (9953, 176, 4, 200, 16, 8, '2020-12-16 10:04:45');
INSERT INTO `stock_log` VALUES (9954, 174, 4, 204, 18, 9, '2020-12-16 10:08:46');
INSERT INTO `stock_log` VALUES (9955, 174, 4, 204, 1, 1, '2020-12-16 10:10:24');
INSERT INTO `stock_log` VALUES (9956, 163, 4, 204, 49, 10, '2020-12-16 10:11:07');
INSERT INTO `stock_log` VALUES (9957, 176, 4, 200, 15, 8, '2020-12-16 10:15:20');
INSERT INTO `stock_log` VALUES (9958, 178, 4, 204, 7, 4, '2020-12-16 10:16:42');
INSERT INTO `stock_log` VALUES (9959, 179, 4, 204, 3, 2, '2020-12-16 10:17:27');
INSERT INTO `stock_log` VALUES (9960, 174, 4, 204, 17, 9, '2020-12-16 10:17:56');
INSERT INTO `stock_log` VALUES (9961, 176, 4, 204, 12, 6, '2020-12-16 10:18:05');
INSERT INTO `stock_log` VALUES (9962, 163, 4, 200, 50, 10, '2020-12-16 10:18:36');
INSERT INTO `stock_log` VALUES (9963, 176, 4, 204, 10, 5, '2020-12-16 10:22:49');
INSERT INTO `stock_log` VALUES (9964, 174, 4, 204, 18, 9, '2020-12-16 10:25:27');
INSERT INTO `stock_log` VALUES (9965, 163, 4, 204, 49, 10, '2020-12-16 10:26:53');
INSERT INTO `stock_log` VALUES (9966, 178, 4, 204, 6, 3, '2020-12-16 10:27:18');
INSERT INTO `stock_log` VALUES (9967, 176, 4, 200, 16, 8, '2020-12-16 10:27:51');
INSERT INTO `stock_log` VALUES (9968, 163, 4, 200, 50, 10, '2020-12-16 10:35:14');
INSERT INTO `stock_log` VALUES (9969, 174, 4, 204, 17, 9, '2020-12-16 10:35:28');
INSERT INTO `stock_log` VALUES (9970, 176, 4, 200, 15, 8, '2020-12-16 10:37:06');
INSERT INTO `stock_log` VALUES (9971, 178, 4, 204, 1, 1, '2020-12-16 10:38:49');
INSERT INTO `stock_log` VALUES (9972, 176, 4, 204, 11, 6, '2020-12-16 10:40:19');
INSERT INTO `stock_log` VALUES (9973, 174, 4, 204, 18, 9, '2020-12-16 10:43:17');
INSERT INTO `stock_log` VALUES (9974, 176, 4, 204, 9, 5, '2020-12-16 10:43:36');
INSERT INTO `stock_log` VALUES (9975, 163, 4, 204, 49, 10, '2020-12-16 10:46:42');
INSERT INTO `stock_log` VALUES (9976, 174, 4, 204, 17, 9, '2020-12-16 10:51:05');
INSERT INTO `stock_log` VALUES (9977, 176, 4, 200, 16, 8, '2020-12-16 10:52:27');
INSERT INTO `stock_log` VALUES (9978, 163, 4, 200, 50, 10, '2020-12-16 10:54:59');
INSERT INTO `stock_log` VALUES (9979, 178, 4, 204, 2, 1, '2020-12-16 10:56:12');
INSERT INTO `stock_log` VALUES (9980, 174, 4, 204, 18, 9, '2020-12-16 10:59:24');
INSERT INTO `stock_log` VALUES (9981, 178, 4, 204, 8, 4, '2020-12-16 11:00:51');
INSERT INTO `stock_log` VALUES (9982, 163, 4, 204, 49, 10, '2020-12-16 11:04:02');
INSERT INTO `stock_log` VALUES (9983, 176, 4, 200, 15, 8, '2020-12-16 11:04:28');
INSERT INTO `stock_log` VALUES (9984, 178, 4, 204, 5, 3, '2020-12-16 11:05:27');
INSERT INTO `stock_log` VALUES (9985, 174, 4, 204, 17, 9, '2020-12-16 11:07:25');
INSERT INTO `stock_log` VALUES (9986, 176, 4, 204, 10, 5, '2020-12-16 11:08:34');
INSERT INTO `stock_log` VALUES (9987, 178, 4, 204, 1, 1, '2020-12-16 11:08:36');
INSERT INTO `stock_log` VALUES (9988, 163, 4, 200, 50, 10, '2020-12-16 11:11:44');
INSERT INTO `stock_log` VALUES (9989, 176, 4, 204, 12, 6, '2020-12-16 11:12:34');
INSERT INTO `stock_log` VALUES (9990, 174, 4, 204, 18, 9, '2020-12-16 11:15:01');
INSERT INTO `stock_log` VALUES (9991, 176, 4, 200, 16, 8, '2020-12-16 11:16:04');
INSERT INTO `stock_log` VALUES (9992, 163, 4, 204, 49, 10, '2020-12-16 11:19:38');
INSERT INTO `stock_log` VALUES (9993, 174, 4, 204, 17, 9, '2020-12-16 11:23:06');
INSERT INTO `stock_log` VALUES (9994, 178, 4, 204, 2, 1, '2020-12-16 11:24:27');
INSERT INTO `stock_log` VALUES (9995, 176, 4, 200, 15, 8, '2020-12-16 11:26:37');
INSERT INTO `stock_log` VALUES (9996, 163, 4, 200, 50, 10, '2020-12-16 11:28:21');
INSERT INTO `stock_log` VALUES (9997, 178, 4, 204, 6, 3, '2020-12-16 11:29:11');
INSERT INTO `stock_log` VALUES (9998, 176, 4, 204, 11, 6, '2020-12-16 11:30:19');
INSERT INTO `stock_log` VALUES (9999, 174, 4, 204, 18, 9, '2020-12-16 11:31:17');
INSERT INTO `stock_log` VALUES (10000, 178, 4, 204, 7, 4, '2020-12-16 11:33:47');
INSERT INTO `stock_log` VALUES (10001, 176, 4, 204, 9, 5, '2020-12-16 11:34:35');
INSERT INTO `stock_log` VALUES (10002, 163, 4, 204, 49, 10, '2020-12-16 11:36:31');
INSERT INTO `stock_log` VALUES (10003, 178, 4, 204, 1, 1, '2020-12-16 11:37:47');
INSERT INTO `stock_log` VALUES (10004, 176, 4, 200, 16, 8, '2020-12-16 11:38:47');
INSERT INTO `stock_log` VALUES (10005, 174, 4, 204, 17, 9, '2020-12-16 11:38:57');
INSERT INTO `stock_log` VALUES (10006, 163, 4, 200, 50, 10, '2020-12-16 11:47:00');
INSERT INTO `stock_log` VALUES (10007, 174, 4, 204, 18, 9, '2020-12-16 11:47:08');
INSERT INTO `stock_log` VALUES (10008, 176, 4, 200, 15, 8, '2020-12-16 11:48:24');
INSERT INTO `stock_log` VALUES (10009, 178, 4, 204, 2, 1, '2020-12-16 11:49:50');
INSERT INTO `stock_log` VALUES (10010, 176, 4, 204, 10, 5, '2020-12-16 11:52:27');
INSERT INTO `stock_log` VALUES (10011, 178, 4, 204, 5, 3, '2020-12-16 11:53:24');
INSERT INTO `stock_log` VALUES (10012, 174, 4, 204, 17, 9, '2020-12-16 11:55:06');
INSERT INTO `stock_log` VALUES (10013, 163, 4, 204, 49, 10, '2020-12-16 11:56:36');
INSERT INTO `stock_log` VALUES (10014, 176, 4, 204, 12, 6, '2020-12-16 11:56:54');
INSERT INTO `stock_log` VALUES (10015, 178, 4, 204, 8, 4, '2020-12-16 11:57:21');
INSERT INTO `stock_log` VALUES (10016, 176, 4, 200, 16, 8, '2020-12-16 12:00:46');
INSERT INTO `stock_log` VALUES (10017, 163, 4, 200, 50, 10, '2020-12-16 12:04:18');
INSERT INTO `stock_log` VALUES (10018, 174, 4, 204, 18, 9, '2020-12-16 12:05:56');
INSERT INTO `stock_log` VALUES (10019, 176, 4, 200, 15, 8, '2020-12-16 12:10:11');
INSERT INTO `stock_log` VALUES (10020, 163, 4, 204, 49, 10, '2020-12-16 12:13:02');
INSERT INTO `stock_log` VALUES (10021, 178, 4, 204, 1, 1, '2020-12-16 12:14:07');
INSERT INTO `stock_log` VALUES (10022, 174, 4, 204, 17, 9, '2020-12-16 12:14:49');
INSERT INTO `stock_log` VALUES (10023, 176, 4, 204, 9, 5, '2020-12-16 12:16:14');
INSERT INTO `stock_log` VALUES (10024, 176, 4, 204, 11, 6, '2020-12-16 12:20:12');
INSERT INTO `stock_log` VALUES (10025, 163, 4, 200, 50, 10, '2020-12-16 12:21:45');
INSERT INTO `stock_log` VALUES (10026, 174, 4, 204, 18, 9, '2020-12-16 12:22:29');
INSERT INTO `stock_log` VALUES (10027, 176, 4, 200, 16, 8, '2020-12-16 12:23:41');
INSERT INTO `stock_log` VALUES (10028, 163, 4, 204, 49, 10, '2020-12-16 12:24:38');
INSERT INTO `stock_log` VALUES (10029, 178, 4, 204, 2, 1, '2020-12-16 12:25:48');
INSERT INTO `stock_log` VALUES (10030, 178, 4, 204, 7, 4, '2020-12-16 12:30:32');
INSERT INTO `stock_log` VALUES (10031, 174, 4, 204, 17, 9, '2020-12-16 12:31:08');
INSERT INTO `stock_log` VALUES (10032, 164, 4, 204, 50, 10, '2020-12-16 12:32:00');
INSERT INTO `stock_log` VALUES (10033, 178, 4, 204, 6, 3, '2020-12-16 12:33:05');
INSERT INTO `stock_log` VALUES (10034, 176, 4, 200, 15, 8, '2020-12-16 12:35:58');
INSERT INTO `stock_log` VALUES (10035, 176, 4, 204, 10, 5, '2020-12-16 12:39:58');
INSERT INTO `stock_log` VALUES (10036, 164, 4, 204, 49, 10, '2020-12-16 12:40:19');
INSERT INTO `stock_log` VALUES (10037, 178, 4, 204, 1, 1, '2020-12-16 12:40:21');
INSERT INTO `stock_log` VALUES (10038, 174, 4, 204, 18, 9, '2020-12-16 12:40:59');
INSERT INTO `stock_log` VALUES (10039, 176, 4, 204, 12, 6, '2020-12-16 12:43:26');
INSERT INTO `stock_log` VALUES (10040, 164, 4, 204, 50, 10, '2020-12-16 12:48:17');
INSERT INTO `stock_log` VALUES (10041, 176, 4, 200, 16, 8, '2020-12-16 12:48:37');
INSERT INTO `stock_log` VALUES (10042, 174, 4, 204, 17, 9, '2020-12-16 12:48:37');
INSERT INTO `stock_log` VALUES (10043, 178, 4, 204, 2, 1, '2020-12-16 12:56:08');
INSERT INTO `stock_log` VALUES (10044, 174, 4, 204, 18, 9, '2020-12-16 12:56:23');
INSERT INTO `stock_log` VALUES (10045, 176, 4, 200, 15, 8, '2020-12-16 12:57:25');
INSERT INTO `stock_log` VALUES (10046, 178, 4, 204, 5, 3, '2020-12-16 13:01:12');
INSERT INTO `stock_log` VALUES (10047, 176, 4, 204, 9, 5, '2020-12-16 13:01:56');
INSERT INTO `stock_log` VALUES (10048, 178, 4, 204, 8, 4, '2020-12-16 13:04:46');
INSERT INTO `stock_log` VALUES (10049, 174, 4, 204, 17, 9, '2020-12-16 13:04:55');
INSERT INTO `stock_log` VALUES (10050, 176, 4, 204, 11, 6, '2020-12-16 13:05:57');
INSERT INTO `stock_log` VALUES (10051, 178, 4, 204, 1, 1, '2020-12-16 13:08:43');
INSERT INTO `stock_log` VALUES (10052, 176, 4, 200, 16, 8, '2020-12-16 13:10:36');
INSERT INTO `stock_log` VALUES (10053, 174, 4, 204, 18, 9, '2020-12-16 13:13:10');
INSERT INTO `stock_log` VALUES (10054, 176, 4, 200, 49, 10, '2020-12-16 13:14:41');
INSERT INTO `stock_log` VALUES (10055, 177, 4, 204, 13, 7, '2020-12-16 13:14:48');
INSERT INTO `stock_log` VALUES (10056, 174, 4, 204, 17, 9, '2020-12-16 13:21:45');
INSERT INTO `stock_log` VALUES (10057, 178, 4, 204, 2, 1, '2020-12-16 13:21:54');
INSERT INTO `stock_log` VALUES (10058, 176, 4, 200, 15, 8, '2020-12-16 13:21:55');
INSERT INTO `stock_log` VALUES (10059, 179, 4, 204, 4, 2, '2020-12-16 13:22:20');
INSERT INTO `stock_log` VALUES (10060, 176, 4, 200, 50, 10, '2020-12-16 13:22:56');
INSERT INTO `stock_log` VALUES (10061, 176, 4, 204, 10, 5, '2020-12-16 13:26:05');
INSERT INTO `stock_log` VALUES (10062, 178, 4, 204, 7, 4, '2020-12-16 13:27:59');
INSERT INTO `stock_log` VALUES (10063, 174, 4, 204, 18, 9, '2020-12-16 13:29:17');
INSERT INTO `stock_log` VALUES (10064, 176, 4, 204, 12, 6, '2020-12-16 13:30:52');
INSERT INTO `stock_log` VALUES (10065, 176, 4, 204, 49, 10, '2020-12-16 13:32:13');
INSERT INTO `stock_log` VALUES (10066, 176, 4, 200, 16, 8, '2020-12-16 13:35:59');
INSERT INTO `stock_log` VALUES (10067, 174, 4, 204, 17, 9, '2020-12-16 13:39:06');
INSERT INTO `stock_log` VALUES (10068, 176, 4, 204, 50, 10, '2020-12-16 13:39:18');
INSERT INTO `stock_log` VALUES (10069, 178, 4, 204, 1, 1, '2020-12-16 13:39:39');
INSERT INTO `stock_log` VALUES (10070, 178, 4, 204, 6, 3, '2020-12-16 13:42:50');
INSERT INTO `stock_log` VALUES (10071, 176, 4, 200, 15, 8, '2020-12-16 13:45:35');
INSERT INTO `stock_log` VALUES (10072, 174, 4, 204, 18, 9, '2020-12-16 13:47:01');
INSERT INTO `stock_log` VALUES (10073, 178, 4, 204, 8, 4, '2020-12-16 13:47:13');
INSERT INTO `stock_log` VALUES (10074, 176, 4, 200, 49, 10, '2020-12-16 13:47:15');
INSERT INTO `stock_log` VALUES (10075, 176, 4, 204, 9, 5, '2020-12-16 13:50:00');
INSERT INTO `stock_log` VALUES (10076, 178, 4, 204, 5, 3, '2020-12-16 13:52:25');
INSERT INTO `stock_log` VALUES (10077, 176, 4, 204, 11, 6, '2020-12-16 13:54:14');
INSERT INTO `stock_log` VALUES (10078, 178, 4, 204, 2, 1, '2020-12-16 13:56:45');
INSERT INTO `stock_log` VALUES (10079, 174, 4, 204, 17, 9, '2020-12-16 13:57:34');
INSERT INTO `stock_log` VALUES (10080, 176, 4, 200, 50, 10, '2020-12-16 14:01:18');
INSERT INTO `stock_log` VALUES (10081, 178, 4, 204, 1, 1, '2020-12-16 14:01:51');
INSERT INTO `stock_log` VALUES (10082, 174, 4, 204, 18, 9, '2020-12-16 14:04:34');
INSERT INTO `stock_log` VALUES (10083, 176, 4, 204, 49, 10, '2020-12-16 14:08:21');
INSERT INTO `stock_log` VALUES (10084, 176, 4, 200, 16, 8, '2020-12-16 14:10:48');
INSERT INTO `stock_log` VALUES (10085, 174, 4, 204, 17, 9, '2020-12-16 14:13:37');
INSERT INTO `stock_log` VALUES (10086, 178, 4, 204, 7, 4, '2020-12-16 14:15:34');
INSERT INTO `stock_log` VALUES (10087, 176, 4, 204, 12, 6, '2020-12-16 14:15:40');
INSERT INTO `stock_log` VALUES (10088, 176, 4, 204, 50, 10, '2020-12-16 14:15:48');
INSERT INTO `stock_log` VALUES (10089, 180, 4, 204, 10, 5, '2020-12-16 14:16:07');
INSERT INTO `stock_log` VALUES (10090, 178, 4, 204, 2, 1, '2020-12-16 14:18:25');
INSERT INTO `stock_log` VALUES (10091, 174, 4, 204, 18, 9, '2020-12-16 14:21:19');
INSERT INTO `stock_log` VALUES (10092, 178, 4, 204, 6, 3, '2020-12-16 14:23:40');
INSERT INTO `stock_log` VALUES (10093, 176, 4, 200, 49, 10, '2020-12-16 14:24:03');
INSERT INTO `stock_log` VALUES (10094, 178, 4, 204, 1, 1, '2020-12-16 14:28:46');
INSERT INTO `stock_log` VALUES (10095, 174, 4, 204, 17, 9, '2020-12-16 14:29:57');
INSERT INTO `stock_log` VALUES (10096, 176, 4, 200, 50, 10, '2020-12-16 14:32:02');
INSERT INTO `stock_log` VALUES (10097, 180, 4, 200, 15, 8, '2020-12-16 14:37:38');
INSERT INTO `stock_log` VALUES (10098, 174, 4, 204, 18, 9, '2020-12-16 14:37:54');
INSERT INTO `stock_log` VALUES (10099, 178, 4, 204, 8, 4, '2020-12-16 14:38:34');
INSERT INTO `stock_log` VALUES (10100, 176, 4, 204, 49, 10, '2020-12-16 14:39:09');
INSERT INTO `stock_log` VALUES (10101, 180, 4, 204, 9, 5, '2020-12-16 14:41:40');
INSERT INTO `stock_log` VALUES (10102, 178, 4, 204, 2, 1, '2020-12-16 14:42:49');
INSERT INTO `stock_log` VALUES (10103, 178, 4, 204, 5, 3, '2020-12-16 14:46:19');
INSERT INTO `stock_log` VALUES (10104, 176, 4, 204, 50, 10, '2020-12-16 14:48:42');
INSERT INTO `stock_log` VALUES (10105, 174, 4, 204, 17, 9, '2020-12-16 14:49:19');
INSERT INTO `stock_log` VALUES (10106, 178, 4, 204, 1, 1, '2020-12-16 14:51:42');
INSERT INTO `stock_log` VALUES (10107, 180, 4, 200, 16, 8, '2020-12-16 14:53:35');
INSERT INTO `stock_log` VALUES (10108, 179, 4, 204, 14, 7, '2020-12-16 14:54:21');
INSERT INTO `stock_log` VALUES (10109, 176, 4, 200, 49, 10, '2020-12-16 14:56:19');
INSERT INTO `stock_log` VALUES (10110, 174, 4, 204, 18, 9, '2020-12-16 14:57:18');
INSERT INTO `stock_log` VALUES (10111, 180, 4, 204, 10, 5, '2020-12-16 14:57:27');
INSERT INTO `stock_log` VALUES (10112, 178, 4, 204, 7, 4, '2020-12-16 15:01:26');
INSERT INTO `stock_log` VALUES (10113, 176, 4, 200, 50, 10, '2020-12-16 15:04:05');
INSERT INTO `stock_log` VALUES (10114, 174, 4, 204, 17, 9, '2020-12-16 15:05:34');
INSERT INTO `stock_log` VALUES (10115, 178, 4, 204, 6, 3, '2020-12-16 15:05:37');
INSERT INTO `stock_log` VALUES (10116, 178, 4, 204, 2, 1, '2020-12-16 15:09:52');
INSERT INTO `stock_log` VALUES (10117, 176, 4, 204, 49, 10, '2020-12-16 15:11:07');
INSERT INTO `stock_log` VALUES (10118, 174, 4, 204, 18, 9, '2020-12-16 15:13:27');
INSERT INTO `stock_log` VALUES (10119, 178, 4, 204, 1, 1, '2020-12-16 15:14:20');
INSERT INTO `stock_log` VALUES (10120, 176, 4, 204, 50, 10, '2020-12-16 15:21:27');
INSERT INTO `stock_log` VALUES (10121, 176, 4, 200, 49, 10, '2020-12-16 15:29:44');
INSERT INTO `stock_log` VALUES (10122, 178, 4, 204, 8, 4, '2020-12-16 15:29:52');
INSERT INTO `stock_log` VALUES (10123, 179, 4, 204, 3, 2, '2020-12-16 15:32:41');
INSERT INTO `stock_log` VALUES (10124, 178, 4, 204, 5, 3, '2020-12-16 15:33:58');
INSERT INTO `stock_log` VALUES (10125, 176, 4, 200, 50, 10, '2020-12-16 15:38:16');
INSERT INTO `stock_log` VALUES (10126, 170, 4, 204, 17, 9, '2020-12-16 15:38:41');
INSERT INTO `stock_log` VALUES (10127, 178, 4, 204, 2, 1, '2020-12-16 15:40:26');
INSERT INTO `stock_log` VALUES (10128, 183, 4, 204, 18, 9, '2020-12-16 15:45:53');
INSERT INTO `stock_log` VALUES (10129, 178, 4, 204, 17, 9, '2020-12-16 15:49:11');
INSERT INTO `stock_log` VALUES (10130, 176, 4, 204, 49, 10, '2020-12-16 15:49:29');
INSERT INTO `stock_log` VALUES (10131, 178, 4, 204, 1, 1, '2020-12-16 15:52:03');
INSERT INTO `stock_log` VALUES (10132, 181, 4, 200, 15, 8, '2020-12-16 15:52:40');
INSERT INTO `stock_log` VALUES (10133, 178, 4, 204, 18, 9, '2020-12-16 15:53:38');
INSERT INTO `stock_log` VALUES (10134, 176, 4, 204, 50, 10, '2020-12-16 15:56:39');
INSERT INTO `stock_log` VALUES (10135, 178, 4, 204, 7, 4, '2020-12-16 16:01:56');
INSERT INTO `stock_log` VALUES (10136, 178, 4, 204, 17, 9, '2020-12-16 16:06:47');
INSERT INTO `stock_log` VALUES (10137, 178, 4, 204, 6, 3, '2020-12-16 16:06:47');
INSERT INTO `stock_log` VALUES (10138, 181, 4, 204, 9, 5, '2020-12-16 16:06:50');
INSERT INTO `stock_log` VALUES (10139, 176, 4, 200, 49, 10, '2020-12-16 16:06:50');
INSERT INTO `stock_log` VALUES (10140, 178, 4, 204, 2, 1, '2020-12-16 16:11:35');
INSERT INTO `stock_log` VALUES (10141, 176, 4, 200, 50, 10, '2020-12-16 16:13:34');
INSERT INTO `stock_log` VALUES (10142, 178, 4, 204, 18, 9, '2020-12-16 16:14:13');
INSERT INTO `stock_log` VALUES (10143, 178, 4, 204, 17, 9, '2020-12-16 16:22:28');
INSERT INTO `stock_log` VALUES (10144, 180, 4, 204, 11, 6, '2020-12-16 16:23:46');
INSERT INTO `stock_log` VALUES (10145, 184, 4, 204, 1, 1, '2020-12-16 16:26:25');
INSERT INTO `stock_log` VALUES (10146, 178, 4, 204, 18, 9, '2020-12-16 16:31:21');
INSERT INTO `stock_log` VALUES (10147, 184, 4, 204, 8, 4, '2020-12-16 16:35:52');
INSERT INTO `stock_log` VALUES (10148, 176, 4, 204, 49, 10, '2020-12-16 16:37:47');
INSERT INTO `stock_log` VALUES (10149, 178, 4, 204, 17, 9, '2020-12-16 16:39:49');
INSERT INTO `stock_log` VALUES (10150, 184, 4, 204, 5, 3, '2020-12-16 16:40:28');
INSERT INTO `stock_log` VALUES (10151, 176, 4, 204, 50, 10, '2020-12-16 16:41:57');
INSERT INTO `stock_log` VALUES (10152, 184, 4, 204, 2, 1, '2020-12-16 16:43:39');
INSERT INTO `stock_log` VALUES (10153, 178, 4, 204, 18, 9, '2020-12-16 16:47:51');
INSERT INTO `stock_log` VALUES (10154, 176, 4, 200, 49, 10, '2020-12-16 16:48:54');
INSERT INTO `stock_log` VALUES (10155, 184, 4, 204, 1, 1, '2020-12-16 16:51:30');
INSERT INTO `stock_log` VALUES (10156, 176, 4, 200, 50, 10, '2020-12-16 16:55:59');
INSERT INTO `stock_log` VALUES (10157, 178, 4, 204, 17, 9, '2020-12-16 16:57:02');
INSERT INTO `stock_log` VALUES (10158, 184, 4, 204, 7, 4, '2020-12-16 17:00:42');
INSERT INTO `stock_log` VALUES (10159, 176, 4, 204, 49, 10, '2020-12-16 17:04:41');
INSERT INTO `stock_log` VALUES (10160, 184, 4, 204, 2, 1, '2020-12-16 17:04:46');
INSERT INTO `stock_log` VALUES (10161, 178, 4, 204, 18, 9, '2020-12-16 17:05:48');
INSERT INTO `stock_log` VALUES (10162, 184, 4, 204, 6, 3, '2020-12-16 17:09:22');
INSERT INTO `stock_log` VALUES (10163, 178, 4, 204, 17, 9, '2020-12-16 17:14:00');
INSERT INTO `stock_log` VALUES (10164, 184, 4, 204, 1, 1, '2020-12-16 17:14:11');
INSERT INTO `stock_log` VALUES (10165, 176, 4, 204, 50, 10, '2020-12-16 17:15:08');
INSERT INTO `stock_log` VALUES (10166, 178, 4, 204, 18, 9, '2020-12-16 17:21:32');
INSERT INTO `stock_log` VALUES (10167, 176, 4, 200, 49, 10, '2020-12-16 17:23:28');
INSERT INTO `stock_log` VALUES (10168, 184, 4, 204, 2, 1, '2020-12-16 17:27:07');
INSERT INTO `stock_log` VALUES (10169, 176, 4, 200, 50, 10, '2020-12-16 17:31:04');
INSERT INTO `stock_log` VALUES (10170, 178, 4, 204, 17, 9, '2020-12-16 17:31:20');
INSERT INTO `stock_log` VALUES (10171, 184, 4, 204, 5, 3, '2020-12-16 17:31:50');
INSERT INTO `stock_log` VALUES (10172, 184, 4, 204, 1, 1, '2020-12-16 17:37:18');
INSERT INTO `stock_log` VALUES (10173, 178, 4, 204, 18, 9, '2020-12-16 17:39:25');
INSERT INTO `stock_log` VALUES (10174, 176, 4, 204, 49, 10, '2020-12-16 17:42:19');
INSERT INTO `stock_log` VALUES (10175, 178, 4, 204, 17, 9, '2020-12-16 17:47:40');
INSERT INTO `stock_log` VALUES (10176, 176, 4, 204, 50, 10, '2020-12-16 17:49:45');
INSERT INTO `stock_log` VALUES (10177, 184, 4, 204, 7, 4, '2020-12-16 17:49:50');
INSERT INTO `stock_log` VALUES (10178, 178, 4, 204, 18, 9, '2020-12-16 17:56:54');
INSERT INTO `stock_log` VALUES (10179, 184, 4, 204, 6, 3, '2020-12-16 17:56:58');
INSERT INTO `stock_log` VALUES (10180, 176, 4, 200, 49, 10, '2020-12-16 17:57:42');
INSERT INTO `stock_log` VALUES (10181, 184, 4, 204, 2, 1, '2020-12-16 18:00:38');
INSERT INTO `stock_log` VALUES (10182, 178, 4, 204, 17, 9, '2020-12-16 18:04:19');
INSERT INTO `stock_log` VALUES (10183, 184, 4, 204, 1, 1, '2020-12-16 18:05:21');
INSERT INTO `stock_log` VALUES (10184, 176, 4, 200, 50, 10, '2020-12-16 18:05:57');
INSERT INTO `stock_log` VALUES (10185, 176, 4, 204, 49, 10, '2020-12-16 18:13:03');
INSERT INTO `stock_log` VALUES (10186, 178, 4, 204, 18, 9, '2020-12-16 18:13:50');
INSERT INTO `stock_log` VALUES (10187, 176, 4, 204, 50, 10, '2020-12-16 18:21:01');
INSERT INTO `stock_log` VALUES (10188, 178, 4, 204, 17, 9, '2020-12-16 18:21:32');
INSERT INTO `stock_log` VALUES (10189, 176, 4, 200, 49, 10, '2020-12-16 18:30:52');
INSERT INTO `stock_log` VALUES (10190, 178, 4, 204, 18, 9, '2020-12-16 18:31:50');
INSERT INTO `stock_log` VALUES (10191, 181, 4, 200, 15, 8, '2020-12-16 18:34:43');
INSERT INTO `stock_log` VALUES (10192, 176, 4, 200, 50, 10, '2020-12-16 18:38:40');
INSERT INTO `stock_log` VALUES (10193, 178, 4, 204, 17, 9, '2020-12-16 18:40:12');
INSERT INTO `stock_log` VALUES (10194, 178, 4, 204, 18, 9, '2020-12-16 18:50:38');
INSERT INTO `stock_log` VALUES (10195, 176, 4, 204, 49, 10, '2020-12-16 18:54:33');
INSERT INTO `stock_log` VALUES (10196, 178, 4, 204, 17, 9, '2020-12-16 19:01:59');
INSERT INTO `stock_log` VALUES (10197, 176, 4, 204, 50, 10, '2020-12-16 19:06:52');
INSERT INTO `stock_log` VALUES (10198, 178, 4, 204, 18, 9, '2020-12-16 19:08:38');
INSERT INTO `stock_log` VALUES (10199, 176, 4, 200, 49, 10, '2020-12-16 19:15:40');
INSERT INTO `stock_log` VALUES (10200, 178, 4, 204, 17, 9, '2020-12-16 19:17:29');
INSERT INTO `stock_log` VALUES (10201, 176, 4, 200, 50, 10, '2020-12-16 19:23:05');
INSERT INTO `stock_log` VALUES (10202, 178, 4, 204, 18, 9, '2020-12-16 19:26:20');
INSERT INTO `stock_log` VALUES (10203, 176, 4, 204, 49, 10, '2020-12-16 19:30:33');
INSERT INTO `stock_log` VALUES (10204, 178, 4, 204, 17, 9, '2020-12-16 19:34:10');
INSERT INTO `stock_log` VALUES (10205, 178, 4, 204, 18, 9, '2020-12-16 19:41:51');
INSERT INTO `stock_log` VALUES (10206, 176, 4, 204, 50, 10, '2020-12-16 19:42:32');
INSERT INTO `stock_log` VALUES (10207, 176, 4, 200, 49, 10, '2020-12-16 19:50:17');
INSERT INTO `stock_log` VALUES (10208, 178, 4, 204, 17, 9, '2020-12-16 19:50:30');
INSERT INTO `stock_log` VALUES (10209, 178, 4, 204, 18, 9, '2020-12-16 19:58:09');
INSERT INTO `stock_log` VALUES (10210, 185, 4, 204, 4, 2, '2020-12-16 19:59:32');
INSERT INTO `stock_log` VALUES (10211, 184, 4, 204, 5, 3, '2020-12-16 20:03:44');
INSERT INTO `stock_log` VALUES (10212, 176, 4, 200, 50, 10, '2020-12-16 20:11:20');
INSERT INTO `stock_log` VALUES (10213, 184, 4, 204, 8, 4, '2020-12-16 20:13:44');
INSERT INTO `stock_log` VALUES (10214, 178, 4, 204, 17, 9, '2020-12-16 20:16:22');
INSERT INTO `stock_log` VALUES (10215, 176, 4, 204, 49, 10, '2020-12-16 20:21:31');
INSERT INTO `stock_log` VALUES (10216, 178, 4, 204, 18, 9, '2020-12-16 20:25:39');
INSERT INTO `stock_log` VALUES (10217, 176, 4, 204, 50, 10, '2020-12-16 20:31:03');
INSERT INTO `stock_log` VALUES (10218, 178, 4, 204, 17, 9, '2020-12-16 20:34:46');
INSERT INTO `stock_log` VALUES (10219, 176, 4, 200, 49, 10, '2020-12-16 20:42:32');
INSERT INTO `stock_log` VALUES (10220, 178, 4, 204, 18, 9, '2020-12-16 20:46:39');
INSERT INTO `stock_log` VALUES (10221, 178, 4, 204, 17, 9, '2020-12-16 20:57:52');
INSERT INTO `stock_log` VALUES (10222, 180, 4, 200, 50, 10, '2020-12-16 20:58:44');
INSERT INTO `stock_log` VALUES (10223, 180, 4, 204, 49, 10, '2020-12-16 21:05:42');
INSERT INTO `stock_log` VALUES (10224, 178, 4, 204, 18, 9, '2020-12-16 21:11:32');
INSERT INTO `stock_log` VALUES (10225, 180, 4, 204, 49, 10, '2020-12-16 21:16:02');
INSERT INTO `stock_log` VALUES (10226, 180, 4, 200, 50, 10, '2020-12-16 21:24:13');
INSERT INTO `stock_log` VALUES (10227, 178, 4, 204, 17, 9, '2020-12-16 21:28:03');
INSERT INTO `stock_log` VALUES (10228, 177, 4, 204, 49, 10, '2020-12-16 21:31:24');
INSERT INTO `stock_log` VALUES (10229, 178, 4, 204, 18, 9, '2020-12-16 21:37:18');
INSERT INTO `stock_log` VALUES (10230, 178, 4, 204, 17, 9, '2020-12-16 21:49:31');
INSERT INTO `stock_log` VALUES (10231, 177, 4, 204, 49, 10, '2020-12-16 21:54:03');
INSERT INTO `stock_log` VALUES (10232, 178, 4, 204, 18, 9, '2020-12-16 22:01:33');
INSERT INTO `stock_log` VALUES (10233, 181, 4, 200, 50, 10, '2020-12-16 22:03:47');
INSERT INTO `stock_log` VALUES (10234, 175, 4, 204, 49, 10, '2020-12-16 22:11:07');
INSERT INTO `stock_log` VALUES (10235, 178, 4, 204, 17, 9, '2020-12-16 22:15:48');
INSERT INTO `stock_log` VALUES (10236, 175, 4, 204, 50, 10, '2020-12-16 22:28:26');
INSERT INTO `stock_log` VALUES (10237, 178, 4, 204, 18, 9, '2020-12-16 22:31:44');
INSERT INTO `stock_log` VALUES (10238, 178, 4, 204, 17, 9, '2020-12-16 22:50:17');
INSERT INTO `stock_log` VALUES (10239, 175, 4, 204, 49, 10, '2020-12-16 22:55:52');
INSERT INTO `stock_log` VALUES (10240, 178, 4, 204, 18, 9, '2020-12-16 22:56:42');
INSERT INTO `stock_log` VALUES (10241, 175, 4, 204, 50, 10, '2020-12-16 23:04:40');
INSERT INTO `stock_log` VALUES (10242, 178, 4, 204, 17, 9, '2020-12-16 23:07:44');
INSERT INTO `stock_log` VALUES (10243, 175, 4, 204, 49, 10, '2020-12-16 23:11:59');
INSERT INTO `stock_log` VALUES (10244, 178, 4, 204, 18, 9, '2020-12-16 23:17:30');
INSERT INTO `stock_log` VALUES (10245, 178, 4, 204, 17, 9, '2020-12-16 23:28:55');
INSERT INTO `stock_log` VALUES (10246, 184, 4, 204, 2, 1, '2020-12-16 23:38:35');
INSERT INTO `stock_log` VALUES (10247, 178, 4, 204, 18, 9, '2020-12-16 23:39:28');
INSERT INTO `stock_log` VALUES (10248, 184, 4, 204, 17, 9, '2020-12-17 00:00:20');
INSERT INTO `stock_log` VALUES (10249, 184, 4, 204, 1, 1, '2020-12-17 00:07:04');
INSERT INTO `stock_log` VALUES (10250, 184, 4, 204, 18, 9, '2020-12-17 00:08:45');
INSERT INTO `stock_log` VALUES (10251, 184, 4, 204, 7, 4, '2020-12-17 00:17:50');
INSERT INTO `stock_log` VALUES (10252, 184, 4, 204, 6, 3, '2020-12-17 00:22:05');
INSERT INTO `stock_log` VALUES (10253, 184, 4, 204, 17, 9, '2020-12-17 00:22:41');
INSERT INTO `stock_log` VALUES (10254, 184, 4, 204, 2, 1, '2020-12-17 00:27:43');
INSERT INTO `stock_log` VALUES (10255, 184, 4, 204, 18, 9, '2020-12-17 00:30:52');
INSERT INTO `stock_log` VALUES (10256, 184, 4, 204, 17, 9, '2020-12-17 00:38:49');
INSERT INTO `stock_log` VALUES (10257, 184, 4, 204, 1, 1, '2020-12-17 00:39:33');
INSERT INTO `stock_log` VALUES (10258, 184, 4, 204, 8, 4, '2020-12-17 00:43:10');
INSERT INTO `stock_log` VALUES (10259, 184, 4, 204, 18, 9, '2020-12-17 00:46:58');
INSERT INTO `stock_log` VALUES (10260, 184, 4, 204, 5, 3, '2020-12-17 00:48:16');
INSERT INTO `stock_log` VALUES (10261, 184, 4, 204, 2, 1, '2020-12-17 00:51:56');
INSERT INTO `stock_log` VALUES (10262, 184, 4, 204, 17, 9, '2020-12-17 00:56:40');
INSERT INTO `stock_log` VALUES (10263, 184, 4, 204, 18, 9, '2020-12-17 01:06:03');
INSERT INTO `stock_log` VALUES (10264, 184, 4, 204, 1, 1, '2020-12-17 01:11:19');
INSERT INTO `stock_log` VALUES (10265, 186, 3, 162, 15, 8, '2020-12-17 01:12:53');
INSERT INTO `stock_log` VALUES (10266, 184, 4, 204, 7, 4, '2020-12-17 01:14:11');
INSERT INTO `stock_log` VALUES (10267, 184, 4, 204, 17, 9, '2020-12-17 01:15:21');
INSERT INTO `stock_log` VALUES (10268, 184, 4, 204, 6, 3, '2020-12-17 01:18:40');
INSERT INTO `stock_log` VALUES (10269, 185, 4, 204, 3, 2, '2020-12-17 01:19:06');
INSERT INTO `stock_log` VALUES (10270, 186, 3, 162, 16, 8, '2020-12-17 01:20:50');
INSERT INTO `stock_log` VALUES (10271, 184, 4, 204, 2, 1, '2020-12-17 01:23:45');
INSERT INTO `stock_log` VALUES (10272, 186, 3, 162, 10, 5, '2020-12-17 01:25:29');
INSERT INTO `stock_log` VALUES (10273, 184, 4, 204, 18, 9, '2020-12-17 01:26:18');
INSERT INTO `stock_log` VALUES (10274, 186, 3, 162, 11, 6, '2020-12-17 01:30:43');
INSERT INTO `stock_log` VALUES (10275, 184, 4, 204, 17, 9, '2020-12-17 01:36:15');
INSERT INTO `stock_log` VALUES (10276, 186, 3, 162, 15, 8, '2020-12-17 01:38:09');
INSERT INTO `stock_log` VALUES (10277, 184, 4, 204, 1, 1, '2020-12-17 01:38:28');
INSERT INTO `stock_log` VALUES (10278, 184, 4, 204, 8, 4, '2020-12-17 01:41:37');
INSERT INTO `stock_log` VALUES (10279, 186, 3, 162, 12, 6, '2020-12-17 01:45:47');
INSERT INTO `stock_log` VALUES (10280, 184, 4, 204, 5, 3, '2020-12-17 01:45:48');
INSERT INTO `stock_log` VALUES (10281, 184, 4, 204, 18, 9, '2020-12-17 01:45:55');
INSERT INTO `stock_log` VALUES (10282, 184, 4, 204, 2, 1, '2020-12-17 01:50:13');
INSERT INTO `stock_log` VALUES (10283, 184, 4, 204, 17, 9, '2020-12-17 01:55:47');
INSERT INTO `stock_log` VALUES (10284, 186, 3, 162, 16, 8, '2020-12-17 02:00:38');
INSERT INTO `stock_log` VALUES (10285, 187, 3, 162, 14, 7, '2020-12-17 02:04:03');
INSERT INTO `stock_log` VALUES (10286, 184, 4, 204, 1, 1, '2020-12-17 02:04:50');
INSERT INTO `stock_log` VALUES (10287, 184, 4, 204, 18, 9, '2020-12-17 02:05:07');
INSERT INTO `stock_log` VALUES (10288, 184, 4, 204, 7, 4, '2020-12-17 02:10:08');
INSERT INTO `stock_log` VALUES (10289, 186, 3, 162, 11, 6, '2020-12-17 02:10:16');
INSERT INTO `stock_log` VALUES (10290, 186, 3, 162, 9, 5, '2020-12-17 02:14:17');
INSERT INTO `stock_log` VALUES (10291, 184, 4, 204, 6, 3, '2020-12-17 02:14:38');
INSERT INTO `stock_log` VALUES (10292, 187, 3, 162, 13, 7, '2020-12-17 02:14:42');
INSERT INTO `stock_log` VALUES (10293, 184, 4, 204, 17, 9, '2020-12-17 02:15:49');
INSERT INTO `stock_log` VALUES (10294, 186, 3, 162, 15, 8, '2020-12-17 02:18:01');
INSERT INTO `stock_log` VALUES (10295, 184, 4, 204, 18, 9, '2020-12-17 02:23:31');
INSERT INTO `stock_log` VALUES (10296, 184, 4, 204, 17, 9, '2020-12-17 02:32:26');
INSERT INTO `stock_log` VALUES (10297, 186, 3, 162, 16, 8, '2020-12-17 02:37:42');
INSERT INTO `stock_log` VALUES (10298, 184, 4, 204, 18, 9, '2020-12-17 02:41:29');
INSERT INTO `stock_log` VALUES (10299, 184, 4, 204, 17, 9, '2020-12-17 02:47:35');
INSERT INTO `stock_log` VALUES (10300, 184, 4, 204, 18, 9, '2020-12-17 02:55:04');
INSERT INTO `stock_log` VALUES (10301, 184, 4, 204, 17, 9, '2020-12-17 03:05:00');
INSERT INTO `stock_log` VALUES (10302, 186, 3, 162, 15, 8, '2020-12-17 03:06:57');
INSERT INTO `stock_log` VALUES (10303, 187, 3, 162, 14, 7, '2020-12-17 03:07:00');
INSERT INTO `stock_log` VALUES (10304, 186, 3, 162, 12, 6, '2020-12-17 03:11:11');
INSERT INTO `stock_log` VALUES (10305, 188, 3, 162, 9, 5, '2020-12-17 03:12:15');
INSERT INTO `stock_log` VALUES (10306, 184, 4, 204, 18, 9, '2020-12-17 03:15:06');
INSERT INTO `stock_log` VALUES (10307, 188, 3, 162, 10, 5, '2020-12-17 03:22:32');
INSERT INTO `stock_log` VALUES (10308, 184, 4, 204, 17, 9, '2020-12-17 03:25:38');
INSERT INTO `stock_log` VALUES (10309, 185, 4, 204, 4, 2, '2020-12-17 03:31:41');
INSERT INTO `stock_log` VALUES (10310, 184, 4, 204, 18, 9, '2020-12-17 03:35:29');
INSERT INTO `stock_log` VALUES (10311, 184, 4, 204, 2, 1, '2020-12-17 03:36:37');
INSERT INTO `stock_log` VALUES (10312, 184, 4, 204, 17, 9, '2020-12-17 03:46:26');
INSERT INTO `stock_log` VALUES (10313, 184, 4, 204, 5, 3, '2020-12-17 03:47:07');
INSERT INTO `stock_log` VALUES (10314, 185, 4, 204, 3, 2, '2020-12-17 03:49:45');
INSERT INTO `stock_log` VALUES (10315, 184, 4, 204, 1, 1, '2020-12-17 03:50:20');
INSERT INTO `stock_log` VALUES (10316, 192, 4, 204, 8, 4, '2020-12-17 03:56:15');
INSERT INTO `stock_log` VALUES (10317, 190, 3, 162, 16, 8, '2020-12-17 03:57:58');
INSERT INTO `stock_log` VALUES (10318, 184, 4, 204, 18, 9, '2020-12-17 03:58:11');
INSERT INTO `stock_log` VALUES (10319, 184, 4, 204, 17, 9, '2020-12-17 04:14:20');
INSERT INTO `stock_log` VALUES (10320, 190, 3, 162, 15, 8, '2020-12-17 04:17:12');
INSERT INTO `stock_log` VALUES (10321, 192, 4, 204, 2, 1, '2020-12-17 04:22:46');
INSERT INTO `stock_log` VALUES (10322, 188, 3, 162, 49, 10, '2020-12-17 04:22:47');
INSERT INTO `stock_log` VALUES (10323, 184, 4, 204, 18, 9, '2020-12-17 04:26:54');
INSERT INTO `stock_log` VALUES (10324, 188, 3, 162, 50, 10, '2020-12-17 04:29:09');
INSERT INTO `stock_log` VALUES (10325, 191, 3, 162, 13, 7, '2020-12-17 04:33:30');
INSERT INTO `stock_log` VALUES (10326, 192, 4, 204, 7, 4, '2020-12-17 04:33:58');
INSERT INTO `stock_log` VALUES (10327, 184, 4, 204, 17, 9, '2020-12-17 04:36:52');
INSERT INTO `stock_log` VALUES (10328, 192, 4, 204, 6, 3, '2020-12-17 04:38:05');
INSERT INTO `stock_log` VALUES (10329, 192, 4, 204, 1, 1, '2020-12-17 04:42:12');
INSERT INTO `stock_log` VALUES (10330, 190, 3, 162, 16, 8, '2020-12-17 04:45:25');
INSERT INTO `stock_log` VALUES (10331, 188, 3, 162, 50, 10, '2020-12-17 04:47:11');
INSERT INTO `stock_log` VALUES (10332, 184, 4, 204, 18, 9, '2020-12-17 04:47:21');
INSERT INTO `stock_log` VALUES (10333, 192, 4, 204, 2, 1, '2020-12-17 04:49:21');
INSERT INTO `stock_log` VALUES (10334, 190, 3, 162, 9, 5, '2020-12-17 04:58:43');
INSERT INTO `stock_log` VALUES (10335, 184, 4, 204, 17, 9, '2020-12-17 04:58:46');
INSERT INTO `stock_log` VALUES (10336, 192, 4, 204, 8, 4, '2020-12-17 05:05:39');
INSERT INTO `stock_log` VALUES (10337, 192, 4, 204, 5, 3, '2020-12-17 05:11:01');
INSERT INTO `stock_log` VALUES (10338, 184, 4, 204, 18, 9, '2020-12-17 05:11:14');
INSERT INTO `stock_log` VALUES (10339, 190, 3, 162, 11, 6, '2020-12-17 05:11:32');
INSERT INTO `stock_log` VALUES (10340, 192, 4, 204, 1, 1, '2020-12-17 05:14:50');
INSERT INTO `stock_log` VALUES (10341, 192, 4, 204, 2, 1, '2020-12-17 05:18:06');
INSERT INTO `stock_log` VALUES (10342, 184, 4, 204, 17, 9, '2020-12-17 05:22:40');
INSERT INTO `stock_log` VALUES (10343, 184, 4, 204, 18, 9, '2020-12-17 05:30:47');
INSERT INTO `stock_log` VALUES (10344, 192, 4, 204, 6, 3, '2020-12-17 05:35:46');
INSERT INTO `stock_log` VALUES (10345, 193, 4, 204, 4, 2, '2020-12-17 05:35:49');
INSERT INTO `stock_log` VALUES (10346, 184, 4, 204, 17, 9, '2020-12-17 05:43:25');
INSERT INTO `stock_log` VALUES (10347, 192, 4, 204, 7, 4, '2020-12-17 05:43:59');
INSERT INTO `stock_log` VALUES (10348, 190, 3, 162, 15, 8, '2020-12-17 05:43:59');
INSERT INTO `stock_log` VALUES (10349, 192, 4, 204, 1, 1, '2020-12-17 05:45:38');
INSERT INTO `stock_log` VALUES (10350, 184, 4, 204, 18, 9, '2020-12-17 05:50:13');
INSERT INTO `stock_log` VALUES (10351, 184, 4, 204, 17, 9, '2020-12-17 06:04:40');
INSERT INTO `stock_log` VALUES (10352, 190, 3, 162, 16, 8, '2020-12-17 06:07:06');
INSERT INTO `stock_log` VALUES (10353, 192, 4, 204, 2, 1, '2020-12-17 06:09:22');
INSERT INTO `stock_log` VALUES (10354, 184, 4, 204, 18, 9, '2020-12-17 06:12:37');
INSERT INTO `stock_log` VALUES (10355, 191, 3, 162, 14, 7, '2020-12-17 06:18:11');
INSERT INTO `stock_log` VALUES (10356, 184, 4, 204, 17, 9, '2020-12-17 06:22:05');
INSERT INTO `stock_log` VALUES (10357, 192, 4, 204, 8, 4, '2020-12-17 06:28:42');
INSERT INTO `stock_log` VALUES (10358, 192, 4, 204, 1, 1, '2020-12-17 06:32:37');
INSERT INTO `stock_log` VALUES (10359, 184, 4, 204, 18, 9, '2020-12-17 06:33:23');
INSERT INTO `stock_log` VALUES (10360, 194, 3, 162, 15, 8, '2020-12-17 06:35:22');
INSERT INTO `stock_log` VALUES (10361, 192, 4, 204, 5, 3, '2020-12-17 06:35:59');
INSERT INTO `stock_log` VALUES (10362, 194, 3, 162, 10, 5, '2020-12-17 06:40:35');
INSERT INTO `stock_log` VALUES (10363, 192, 4, 204, 2, 1, '2020-12-17 06:40:59');
INSERT INTO `stock_log` VALUES (10364, 179, 4, 204, 17, 9, '2020-12-17 06:45:36');
INSERT INTO `stock_log` VALUES (10365, 179, 4, 204, 18, 9, '2020-12-17 06:50:01');
INSERT INTO `stock_log` VALUES (10366, 192, 4, 204, 7, 4, '2020-12-17 06:51:45');
INSERT INTO `stock_log` VALUES (10367, 195, 3, 162, 13, 7, '2020-12-17 06:53:57');
INSERT INTO `stock_log` VALUES (10368, 192, 4, 204, 1, 1, '2020-12-17 06:55:51');
INSERT INTO `stock_log` VALUES (10369, 192, 4, 204, 6, 3, '2020-12-17 06:59:34');
INSERT INTO `stock_log` VALUES (10370, 179, 4, 204, 17, 9, '2020-12-17 07:15:16');
INSERT INTO `stock_log` VALUES (10371, 192, 4, 204, 2, 1, '2020-12-17 07:16:33');
INSERT INTO `stock_log` VALUES (10372, 194, 3, 162, 16, 8, '2020-12-17 07:16:33');
INSERT INTO `stock_log` VALUES (10373, 194, 3, 162, 12, 6, '2020-12-17 07:21:28');
INSERT INTO `stock_log` VALUES (10374, 185, 4, 204, 18, 9, '2020-12-17 07:23:03');
INSERT INTO `stock_log` VALUES (10375, 192, 4, 204, 8, 4, '2020-12-17 07:26:58');
INSERT INTO `stock_log` VALUES (10376, 195, 3, 162, 14, 7, '2020-12-17 07:26:59');
INSERT INTO `stock_log` VALUES (10377, 192, 4, 204, 1, 1, '2020-12-17 07:32:30');
INSERT INTO `stock_log` VALUES (10378, 185, 4, 204, 17, 9, '2020-12-17 07:34:03');
INSERT INTO `stock_log` VALUES (10379, 192, 4, 204, 5, 3, '2020-12-17 07:35:38');
INSERT INTO `stock_log` VALUES (10380, 192, 4, 204, 2, 1, '2020-12-17 07:41:39');
INSERT INTO `stock_log` VALUES (10381, 194, 3, 162, 15, 8, '2020-12-17 07:42:43');
INSERT INTO `stock_log` VALUES (10382, 185, 4, 204, 18, 9, '2020-12-17 07:44:03');
INSERT INTO `stock_log` VALUES (10383, 185, 4, 204, 17, 9, '2020-12-17 07:53:42');
INSERT INTO `stock_log` VALUES (10384, 192, 4, 204, 7, 4, '2020-12-17 07:54:47');
INSERT INTO `stock_log` VALUES (10385, 192, 4, 204, 6, 3, '2020-12-17 07:59:31');
INSERT INTO `stock_log` VALUES (10386, 195, 3, 162, 18, 9, '2020-12-17 08:02:40');
INSERT INTO `stock_log` VALUES (10387, 194, 3, 162, 16, 8, '2020-12-17 08:04:16');
INSERT INTO `stock_log` VALUES (10388, 192, 4, 204, 1, 1, '2020-12-17 08:04:27');
INSERT INTO `stock_log` VALUES (10389, 192, 4, 204, 2, 1, '2020-12-17 08:07:39');
INSERT INTO `stock_log` VALUES (10390, 194, 3, 162, 9, 5, '2020-12-17 08:08:38');
INSERT INTO `stock_log` VALUES (10391, 196, 3, 162, 16, 8, '2020-12-17 08:16:51');
INSERT INTO `stock_log` VALUES (10392, 192, 4, 204, 8, 4, '2020-12-17 08:20:48');
INSERT INTO `stock_log` VALUES (10393, 192, 4, 204, 1, 1, '2020-12-17 08:25:20');
INSERT INTO `stock_log` VALUES (10394, 192, 4, 204, 5, 3, '2020-12-17 08:29:13');
INSERT INTO `stock_log` VALUES (10395, 192, 4, 204, 2, 1, '2020-12-17 08:33:57');
INSERT INTO `stock_log` VALUES (10396, 194, 3, 162, 11, 6, '2020-12-17 08:40:53');
INSERT INTO `stock_log` VALUES (10397, 192, 4, 204, 7, 4, '2020-12-17 08:51:14');
INSERT INTO `stock_log` VALUES (10398, 192, 4, 204, 6, 3, '2020-12-17 08:54:39');
INSERT INTO `stock_log` VALUES (10399, 192, 4, 204, 1, 1, '2020-12-17 08:58:04');
INSERT INTO `stock_log` VALUES (10400, 192, 4, 204, 2, 1, '2020-12-17 09:12:23');
INSERT INTO `stock_log` VALUES (10401, 193, 4, 204, 3, 2, '2020-12-17 09:20:36');
INSERT INTO `stock_log` VALUES (10402, 194, 3, 162, 15, 8, '2020-12-17 09:28:13');
INSERT INTO `stock_log` VALUES (10403, 192, 4, 204, 1, 1, '2020-12-17 09:43:37');
INSERT INTO `stock_log` VALUES (10404, 192, 4, 204, 5, 3, '2020-12-17 09:47:04');
INSERT INTO `stock_log` VALUES (10405, 192, 4, 204, 8, 4, '2020-12-17 09:49:54');
INSERT INTO `stock_log` VALUES (10406, 192, 4, 204, 17, 9, '2020-12-17 09:52:28');
INSERT INTO `stock_log` VALUES (10407, 192, 4, 204, 2, 1, '2020-12-17 09:54:55');
INSERT INTO `stock_log` VALUES (10408, 194, 3, 162, 10, 5, '2020-12-17 09:55:20');
INSERT INTO `stock_log` VALUES (10409, 192, 4, 204, 18, 9, '2020-12-17 09:55:00');
INSERT INTO `stock_log` VALUES (10410, 192, 4, 204, 17, 9, '2020-12-17 09:58:22');
INSERT INTO `stock_log` VALUES (10411, 194, 3, 162, 11, 6, '2020-12-17 09:59:27');
INSERT INTO `stock_log` VALUES (10412, 194, 3, 162, 9, 5, '2020-12-17 09:59:27');
INSERT INTO `stock_log` VALUES (10413, 194, 3, 162, 10, 5, '2020-12-17 09:59:27');
INSERT INTO `stock_log` VALUES (10414, 194, 3, 162, 16, 8, '2020-12-17 09:59:27');
INSERT INTO `stock_log` VALUES (10415, 192, 4, 204, 7, 4, '2020-12-17 10:02:47');
INSERT INTO `stock_log` VALUES (10416, 192, 4, 204, 18, 9, '2020-12-17 10:05:53');
INSERT INTO `stock_log` VALUES (10417, 192, 4, 204, 6, 3, '2020-12-17 10:06:13');
INSERT INTO `stock_log` VALUES (10418, 194, 3, 162, 15, 8, '2020-12-17 10:11:26');
INSERT INTO `stock_log` VALUES (10419, 192, 4, 204, 1, 1, '2020-12-17 10:11:46');

-- ----------------------------
-- Table structure for stock_trans
-- ----------------------------
DROP TABLE IF EXISTS `stock_trans`;
CREATE TABLE `stock_trans`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '交易ID',
  `trans_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '交易类型 1下砖交易 2 上砖交易',
  `trans_status` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '交易状态',
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '区域ID',
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
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '库存任务' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_trans
-- ----------------------------
INSERT INTO `stock_trans` VALUES (7543, 1, 7, 1, 159, 4681, 22, 18, 9, 13, 13, 15, '2020-12-16 00:00:26', '2020-12-16 00:00:27', '2020-12-16 00:02:52', b'1', '2020-12-16 00:03:56', NULL);
INSERT INTO `stock_trans` VALUES (7544, 0, 7, 1, 163, 4899, 9, 44, 5, 12, 12, 16, '2020-12-16 00:02:50', '2020-12-16 00:04:58', '2020-12-16 00:07:46', b'1', '2020-12-16 00:07:47', NULL);
INSERT INTO `stock_trans` VALUES (7545, 1, 7, 1, 161, 4766, 38, 50, 10, 13, 13, 17, '2020-12-16 00:05:05', '2020-12-16 00:08:56', '2020-12-16 00:10:10', b'1', '2020-12-16 00:11:15', NULL);
INSERT INTO `stock_trans` VALUES (7546, 0, 7, 1, 163, 4900, 11, 44, 6, 12, 12, 16, '2020-12-16 00:07:47', '2020-12-16 00:09:42', '2020-12-16 00:12:02', b'1', '2020-12-16 00:12:03', NULL);
INSERT INTO `stock_trans` VALUES (7547, 0, 7, 1, 174, 4901, 2, 19, 1, 11, 11, 19, '2020-12-16 00:10:26', '2020-12-16 00:14:00', '2020-12-16 00:16:14', b'1', '2020-12-16 00:16:15', NULL);
INSERT INTO `stock_trans` VALUES (7548, 1, 7, 1, 161, 4814, 38, 49, 10, 13, 13, 17, '2020-12-16 00:16:15', '2020-12-16 00:16:15', '2020-12-16 00:18:08', b'1', '2020-12-16 00:19:18', NULL);
INSERT INTO `stock_trans` VALUES (7549, 0, 7, 1, 174, 4902, 7, 19, 4, 11, 11, 19, '2020-12-16 00:16:16', '2020-12-16 00:18:14', '2020-12-16 00:20:40', b'1', '2020-12-16 00:20:41', NULL);
INSERT INTO `stock_trans` VALUES (7550, 0, 7, 1, 174, 4903, 6, 19, 3, 11, 11, 19, '2020-12-16 00:20:43', '2020-12-16 00:23:03', '2020-12-16 00:25:21', b'1', '2020-12-16 00:25:22', NULL);
INSERT INTO `stock_trans` VALUES (7551, 1, 7, 1, 159, 4732, 22, 17, 9, 13, 13, 15, '2020-12-16 00:24:02', '2020-12-16 00:24:03', '2020-12-16 00:25:51', b'1', '2020-12-16 00:26:50', NULL);
INSERT INTO `stock_trans` VALUES (7552, 0, 7, 1, 174, 4904, 1, 19, 1, 11, 11, 19, '2020-12-16 00:25:22', '2020-12-16 00:26:47', '2020-12-16 00:28:05', b'1', '2020-12-16 00:28:06', NULL);
INSERT INTO `stock_trans` VALUES (7553, 1, 7, 1, 161, 4856, 38, 50, 10, 13, 13, 17, '2020-12-16 00:29:16', '2020-12-16 00:29:17', '2020-12-16 00:31:03', b'1', '2020-12-16 00:32:07', NULL);
INSERT INTO `stock_trans` VALUES (7554, 0, 7, 1, 163, 4894, 16, 44, 8, 12, 12, 16, '2020-12-16 00:31:45', '2020-12-16 00:33:51', '2020-12-16 00:35:43', b'1', '2020-12-16 00:35:44', NULL);
INSERT INTO `stock_trans` VALUES (7555, 0, 7, 1, 174, 4905, 2, 20, 1, 11, 11, 19, '2020-12-16 00:37:16', '2020-12-16 00:39:19', '2020-12-16 00:41:47', b'1', '2020-12-16 00:41:48', NULL);
INSERT INTO `stock_trans` VALUES (7556, 1, 7, 1, 161, 4862, 38, 49, 10, 13, 13, 17, '2020-12-16 00:40:01', '2020-12-16 00:40:01', '2020-12-16 00:41:58', b'1', '2020-12-16 00:43:22', NULL);
INSERT INTO `stock_trans` VALUES (7557, 0, 7, 1, 174, 4906, 8, 20, 4, 11, 11, 19, '2020-12-16 00:41:49', '2020-12-16 00:44:17', '2020-12-16 00:47:03', b'1', '2020-12-16 00:47:04', NULL);
INSERT INTO `stock_trans` VALUES (7558, 1, 7, 1, 159, 4767, 22, 18, 9, 13, 13, 15, '2020-12-16 00:42:05', '2020-12-16 00:42:06', '2020-12-16 00:45:35', b'1', '2020-12-16 00:46:40', NULL);
INSERT INTO `stock_trans` VALUES (7559, 0, 7, 1, 163, 4907, 15, 44, 8, 12, 12, 16, '2020-12-16 00:46:06', '2020-12-16 00:47:39', '2020-12-16 00:49:06', b'1', '2020-12-16 00:49:07', NULL);
INSERT INTO `stock_trans` VALUES (7560, 0, 7, 1, 174, 4908, 5, 20, 3, 11, 11, 19, '2020-12-16 00:47:05', '2020-12-16 00:48:54', '2020-12-16 00:51:03', b'1', '2020-12-16 00:51:04', NULL);
INSERT INTO `stock_trans` VALUES (7561, 0, 7, 1, 163, 4909, 10, 43, 5, 12, 12, 16, '2020-12-16 00:49:59', '2020-12-16 00:52:39', '2020-12-16 00:55:42', b'1', '2020-12-16 00:55:43', NULL);
INSERT INTO `stock_trans` VALUES (7562, 1, 7, 1, 163, 4872, 33, 50, 10, 13, 13, 19, '2020-12-16 00:51:48', '2020-12-16 00:55:12', '2020-12-16 00:56:17', b'1', '2020-12-16 00:57:13', NULL);
INSERT INTO `stock_trans` VALUES (7563, 0, 7, 1, 174, 4910, 1, 20, 1, 11, 11, 17, '2020-12-16 00:52:42', '2020-12-16 00:57:15', '2020-12-16 00:59:02', b'1', '2020-12-16 00:59:03', NULL);
INSERT INTO `stock_trans` VALUES (7564, 1, 7, 1, 159, 4828, 22, 17, 9, 13, 13, 15, '2020-12-16 00:52:53', '2020-12-16 00:52:53', '2020-12-16 00:59:06', b'1', '2020-12-16 01:00:25', NULL);
INSERT INTO `stock_trans` VALUES (7565, 0, 7, 1, 163, 4911, 12, 43, 6, 12, 12, 16, '2020-12-16 00:55:43', '2020-12-16 00:58:06', '2020-12-16 01:00:46', b'1', '2020-12-16 01:00:47', NULL);
INSERT INTO `stock_trans` VALUES (7566, 1, 7, 1, 163, 4875, 33, 49, 10, 13, 13, 19, '2020-12-16 00:57:17', '2020-12-16 00:57:44', '2020-12-16 01:02:01', b'1', '2020-12-16 01:02:54', NULL);
INSERT INTO `stock_trans` VALUES (7567, 0, 7, 1, 163, 4912, 16, 43, 8, 12, 12, 16, '2020-12-16 01:00:47', '2020-12-16 01:02:55', '2020-12-16 01:05:01', b'1', '2020-12-16 01:05:02', NULL);
INSERT INTO `stock_trans` VALUES (7568, 0, 7, 1, 174, 4913, 2, 20, 1, 11, 11, 17, '2020-12-16 01:08:29', '2020-12-16 01:10:35', '2020-12-16 01:12:12', b'1', '2020-12-16 01:12:13', NULL);
INSERT INTO `stock_trans` VALUES (7569, 0, 7, 1, 163, 4914, 15, 43, 8, 12, 12, 16, '2020-12-16 01:09:14', '2020-12-16 01:10:41', '2020-12-16 01:12:24', b'1', '2020-12-16 01:12:25', NULL);
INSERT INTO `stock_trans` VALUES (7570, 1, 7, 1, 163, 4877, 33, 50, 10, 13, 13, 19, '2020-12-16 01:09:55', '2020-12-16 01:09:56', '2020-12-16 01:11:19', b'1', '2020-12-16 01:12:15', NULL);
INSERT INTO `stock_trans` VALUES (7571, 1, 7, 1, 159, 4880, 27, 18, 9, 13, 13, 15, '2020-12-16 01:11:17', '2020-12-16 01:11:18', '2020-12-16 01:13:57', b'1', '2020-12-16 01:14:57', NULL);
INSERT INTO `stock_trans` VALUES (7572, 0, 7, 1, 174, 4915, 6, 21, 3, 11, 11, 17, '2020-12-16 01:12:13', '2020-12-16 01:14:44', '2020-12-16 01:17:40', b'1', '2020-12-16 01:17:41', NULL);
INSERT INTO `stock_trans` VALUES (7573, 0, 7, 1, 163, 4916, 9, 43, 5, 12, 12, 16, '2020-12-16 01:13:55', '2020-12-16 01:15:57', '2020-12-16 01:17:52', b'1', '2020-12-16 01:17:53', NULL);
INSERT INTO `stock_trans` VALUES (7574, 0, 7, 1, 174, 4917, 7, 21, 4, 11, 11, 17, '2020-12-16 01:17:42', '2020-12-16 01:19:40', '2020-12-16 01:22:22', b'1', '2020-12-16 01:22:23', NULL);
INSERT INTO `stock_trans` VALUES (7575, 0, 7, 1, 163, 4918, 11, 42, 6, 12, 12, 16, '2020-12-16 01:17:55', '2020-12-16 01:19:48', '2020-12-16 01:22:27', b'1', '2020-12-16 01:22:28', NULL);
INSERT INTO `stock_trans` VALUES (7576, 1, 7, 1, 163, 4879, 33, 49, 10, 13, 13, 19, '2020-12-16 01:20:26', '2020-12-16 01:20:26', '2020-12-16 01:21:46', b'1', '2020-12-16 01:22:42', NULL);
INSERT INTO `stock_trans` VALUES (7577, 1, 7, 1, 159, 4888, 27, 17, 9, 13, 13, 15, '2020-12-16 01:21:20', '2020-12-16 01:21:20', '2020-12-16 01:24:35', b'1', '2020-12-16 01:25:57', NULL);
INSERT INTO `stock_trans` VALUES (7578, 0, 7, 1, 163, 4919, 16, 42, 8, 12, 12, 16, '2020-12-16 01:22:28', '2020-12-16 01:24:41', '2020-12-16 01:27:04', b'1', '2020-12-16 01:27:05', NULL);
INSERT INTO `stock_trans` VALUES (7579, 0, 7, 1, 174, 4920, 1, 21, 1, 11, 11, 17, '2020-12-16 01:22:58', '2020-12-16 01:24:23', '2020-12-16 01:26:16', b'1', '2020-12-16 01:26:17', NULL);
INSERT INTO `stock_trans` VALUES (7580, 1, 7, 1, 163, 4893, 33, 50, 10, 13, 13, 19, '2020-12-16 01:31:28', '2020-12-16 01:31:29', '2020-12-16 01:32:53', b'1', '2020-12-16 01:34:24', NULL);
INSERT INTO `stock_trans` VALUES (7581, 0, 7, 1, 163, 4921, 15, 42, 8, 12, 12, 16, '2020-12-16 01:33:17', '2020-12-16 01:34:45', '2020-12-16 01:36:47', b'1', '2020-12-16 01:36:48', NULL);
INSERT INTO `stock_trans` VALUES (7582, 0, 7, 1, 174, 4922, 8, 21, 4, 11, 11, 17, '2020-12-16 01:34:28', '2020-12-16 01:37:05', '2020-12-16 01:39:31', b'1', '2020-12-16 01:39:32', NULL);
INSERT INTO `stock_trans` VALUES (7583, 1, 7, 1, 174, 4898, 19, 18, 9, 13, 13, 15, '2020-12-16 01:37:50', '2020-12-16 01:39:59', '2020-12-16 01:41:19', b'1', '2020-12-16 01:42:29', NULL);
INSERT INTO `stock_trans` VALUES (7584, 0, 7, 1, 163, 4923, 10, 42, 5, 12, 12, 16, '2020-12-16 01:37:55', '2020-12-16 01:40:25', '2020-12-16 01:42:50', b'1', '2020-12-16 01:42:51', NULL);
INSERT INTO `stock_trans` VALUES (7585, 0, 7, 1, 174, 4924, 2, 21, 1, 11, 11, 17, '2020-12-16 01:39:33', '2020-12-16 01:41:36', '2020-12-16 01:43:06', b'1', '2020-12-16 01:43:07', NULL);
INSERT INTO `stock_trans` VALUES (7586, 1, 7, 1, 163, 4896, 44, 49, 10, 13, 13, 19, '2020-12-16 01:41:04', '2020-12-16 01:41:05', '2020-12-16 01:45:31', b'1', '2020-12-16 01:46:59', NULL);
INSERT INTO `stock_trans` VALUES (7587, 1, 7, 1, 174, 4901, 19, 17, 9, 13, 13, 15, '2020-12-16 01:42:30', '2020-12-16 01:43:04', '2020-12-16 01:49:40', b'1', '2020-12-16 01:50:46', NULL);
INSERT INTO `stock_trans` VALUES (7588, 0, 7, 1, 163, 4925, 12, 42, 6, 12, 12, 16, '2020-12-16 01:42:52', '2020-12-16 01:45:13', '2020-12-16 01:47:02', b'1', '2020-12-16 01:47:03', NULL);
INSERT INTO `stock_trans` VALUES (7589, 0, 7, 1, 174, 4926, 5, 22, 3, 11, 11, 17, '2020-12-16 01:43:09', '2020-12-16 01:44:58', '2020-12-16 01:47:34', b'1', '2020-12-16 01:47:35', NULL);
INSERT INTO `stock_trans` VALUES (7590, 0, 7, 1, 163, 4927, 16, 41, 8, 12, 12, 18, '2020-12-16 01:47:03', '2020-12-16 01:49:56', '2020-12-16 01:52:36', b'1', '2020-12-16 01:52:37', NULL);
INSERT INTO `stock_trans` VALUES (7591, 0, 7, 1, 174, 4928, 1, 22, 1, 11, 11, 17, '2020-12-16 01:48:07', '2020-12-16 01:49:36', '2020-12-16 01:51:47', b'1', '2020-12-16 01:51:48', NULL);
INSERT INTO `stock_trans` VALUES (7592, 1, 7, 1, 163, 4899, 44, 50, 10, 13, 13, 19, '2020-12-16 01:51:06', '2020-12-16 01:51:06', '2020-12-16 01:53:54', b'1', '2020-12-16 01:55:13', NULL);
INSERT INTO `stock_trans` VALUES (7593, 0, 7, 1, 175, 4929, 3, 23, 2, 11, 11, 17, '2020-12-16 01:52:52', '2020-12-16 01:54:20', '2020-12-16 01:56:38', b'1', '2020-12-16 01:56:39', NULL);
INSERT INTO `stock_trans` VALUES (7594, 0, 7, 1, 163, 4930, 15, 41, 8, 12, 12, 18, '2020-12-16 01:56:19', '2020-12-16 01:57:50', '2020-12-16 02:08:44', b'1', '2020-12-16 02:08:45', NULL);
INSERT INTO `stock_trans` VALUES (7595, 1, 7, 1, 174, 4902, 19, 18, 9, 13, 13, 15, '2020-12-16 01:58:42', '2020-12-16 01:58:43', '2020-12-16 02:00:52', b'1', '2020-12-16 02:02:01', NULL);
INSERT INTO `stock_trans` VALUES (7596, 1, 7, 1, 163, 4900, 44, 49, 10, 13, 13, 19, '2020-12-16 02:00:19', '2020-12-16 02:00:20', '2020-12-16 02:04:58', b'1', '2020-12-16 02:06:23', NULL);
INSERT INTO `stock_trans` VALUES (7597, 0, 7, 1, 174, 4931, 7, 22, 4, 11, 11, 17, '2020-12-16 02:05:59', '2020-12-16 02:07:52', '2020-12-16 02:10:15', b'1', '2020-12-16 02:10:16', NULL);
INSERT INTO `stock_trans` VALUES (7598, 0, 7, 1, 163, 4932, 11, 41, 6, 12, 12, 18, '2020-12-16 02:08:46', '2020-12-16 02:10:30', '2020-12-16 02:16:16', b'1', '2020-12-16 02:16:17', NULL);
INSERT INTO `stock_trans` VALUES (7599, 1, 7, 1, 163, 4894, 44, 50, 10, 13, 13, 19, '2020-12-16 02:08:50', '2020-12-16 02:08:50', '2020-12-16 02:11:06', b'1', '2020-12-16 02:12:24', NULL);
INSERT INTO `stock_trans` VALUES (7600, 1, 7, 1, 174, 4903, 19, 17, 9, 13, 13, 15, '2020-12-16 02:08:57', '2020-12-16 02:08:57', '2020-12-16 02:15:01', b'1', '2020-12-16 02:16:04', NULL);
INSERT INTO `stock_trans` VALUES (7601, 0, 7, 1, 174, 4933, 2, 22, 1, 11, 11, 17, '2020-12-16 02:10:17', '2020-12-16 02:12:19', '2020-12-16 02:14:04', b'1', '2020-12-16 02:14:05', NULL);
INSERT INTO `stock_trans` VALUES (7602, 0, 7, 1, 174, 4934, 6, 22, 3, 11, 11, 17, '2020-12-16 02:14:07', '2020-12-16 02:16:26', '2020-12-16 02:18:14', b'1', '2020-12-16 02:18:15', NULL);
INSERT INTO `stock_trans` VALUES (7603, 0, 7, 1, 163, 4935, 9, 41, 5, 12, 12, 18, '2020-12-16 02:16:18', '2020-12-16 02:18:12', '2020-12-16 02:20:24', b'1', '2020-12-16 02:20:25', NULL);
INSERT INTO `stock_trans` VALUES (7604, 0, 7, 1, 174, 4936, 1, 24, 1, 11, 11, 17, '2020-12-16 02:19:23', '2020-12-16 02:20:53', '2020-12-16 02:23:21', b'1', '2020-12-16 02:23:22', NULL);
INSERT INTO `stock_trans` VALUES (7605, 1, 7, 1, 163, 4907, 44, 49, 10, 13, 13, 19, '2020-12-16 02:19:43', '2020-12-16 02:19:43', '2020-12-16 02:22:05', b'1', '2020-12-16 02:23:40', NULL);
INSERT INTO `stock_trans` VALUES (7606, 0, 7, 1, 163, 4937, 16, 41, 8, 12, 12, 18, '2020-12-16 02:20:27', '2020-12-16 02:22:37', '2020-12-16 02:31:23', b'1', '2020-12-16 02:31:24', NULL);
INSERT INTO `stock_trans` VALUES (7607, 1, 7, 1, 174, 4904, 19, 18, 9, 13, 13, 15, '2020-12-16 02:20:52', '2020-12-16 02:20:53', '2020-12-16 02:26:21', b'1', '2020-12-16 02:27:44', NULL);
INSERT INTO `stock_trans` VALUES (7608, 0, 7, 1, 164, 4938, 14, 40, 7, 12, 12, 16, '2020-12-16 02:23:14', '2020-12-16 02:26:01', '2020-12-16 02:28:29', b'1', '2020-12-16 02:28:30', NULL);
INSERT INTO `stock_trans` VALUES (7609, 1, 7, 1, 163, 4909, 43, 50, 10, 13, 13, 19, '2020-12-16 02:29:11', '2020-12-16 02:29:12', '2020-12-16 02:31:19', b'1', '2020-12-16 02:32:34', NULL);
INSERT INTO `stock_trans` VALUES (7610, 1, 7, 1, 174, 4905, 20, 17, 9, 13, 13, 15, '2020-12-16 02:30:37', '2020-12-16 02:30:38', '2020-12-16 02:35:03', b'1', '2020-12-16 02:36:04', NULL);
INSERT INTO `stock_trans` VALUES (7611, 0, 7, 1, 174, 4939, 8, 24, 4, 11, 11, 17, '2020-12-16 02:30:43', '2020-12-16 02:33:09', '2020-12-16 02:35:51', b'1', '2020-12-16 02:35:52', NULL);
INSERT INTO `stock_trans` VALUES (7612, 0, 7, 1, 163, 4940, 12, 44, 6, 12, 12, 18, '2020-12-16 02:31:25', '2020-12-16 02:33:43', '2020-12-16 02:36:39', b'1', '2020-12-16 02:36:40', NULL);
INSERT INTO `stock_trans` VALUES (7613, 0, 7, 1, 174, 4941, 5, 24, 3, 11, 11, 17, '2020-12-16 02:35:53', '2020-12-16 02:37:29', '2020-12-16 02:39:29', b'1', '2020-12-16 02:39:30', NULL);
INSERT INTO `stock_trans` VALUES (7614, 0, 7, 1, 163, 4942, 10, 44, 5, 12, 12, 18, '2020-12-16 02:36:41', '2020-12-16 02:39:17', '2020-12-16 02:42:18', b'1', '2020-12-16 02:42:19', NULL);
INSERT INTO `stock_trans` VALUES (7615, 0, 7, 1, 174, 4943, 2, 24, 1, 11, 11, 17, '2020-12-16 02:39:31', '2020-12-16 02:41:38', '2020-12-16 02:43:34', b'1', '2020-12-16 02:43:35', NULL);
INSERT INTO `stock_trans` VALUES (7616, 1, 7, 1, 163, 4911, 43, 49, 10, 13, 13, 19, '2020-12-16 02:39:37', '2020-12-16 02:39:38', '2020-12-16 02:41:56', b'1', '2020-12-16 02:43:17', NULL);
INSERT INTO `stock_trans` VALUES (7617, 1, 7, 1, 174, 4906, 20, 18, 9, 13, 13, 15, '2020-12-16 02:40:59', '2020-12-16 02:41:00', '2020-12-16 02:45:56', b'1', '2020-12-16 02:47:05', NULL);
INSERT INTO `stock_trans` VALUES (7618, 0, 7, 1, 163, 4944, 15, 44, 8, 12, 12, 18, '2020-12-16 02:42:21', '2020-12-16 02:43:50', '2020-12-16 02:45:52', b'1', '2020-12-16 02:45:53', NULL);
INSERT INTO `stock_trans` VALUES (7619, 0, 7, 1, 174, 4945, 1, 24, 1, 11, 11, 17, '2020-12-16 02:43:37', '2020-12-16 02:45:11', '2020-12-16 02:46:36', b'1', '2020-12-16 02:46:37', NULL);
INSERT INTO `stock_trans` VALUES (7620, 0, 7, 1, 163, 4946, 16, 44, 8, 12, 12, 18, '2020-12-16 02:45:55', '2020-12-16 02:47:56', '2020-12-16 02:49:50', b'1', '2020-12-16 02:49:51', NULL);
INSERT INTO `stock_trans` VALUES (7621, 1, 7, 1, 163, 4912, 43, 50, 10, 13, 13, 19, '2020-12-16 02:50:32', '2020-12-16 02:50:32', '2020-12-16 02:52:37', b'1', '2020-12-16 02:53:53', NULL);
INSERT INTO `stock_trans` VALUES (7622, 0, 7, 1, 174, 4947, 7, 19, 4, 11, 11, 17, '2020-12-16 02:53:44', '2020-12-16 02:55:31', '2020-12-16 02:58:24', b'1', '2020-12-16 02:58:25', NULL);
INSERT INTO `stock_trans` VALUES (7623, 1, 7, 1, 174, 4908, 20, 17, 9, 13, 13, 15, '2020-12-16 02:56:19', '2020-12-16 02:56:20', '2020-12-16 02:58:16', b'1', '2020-12-16 02:59:15', NULL);
INSERT INTO `stock_trans` VALUES (7624, 0, 7, 1, 174, 4948, 6, 19, 3, 11, 11, 17, '2020-12-16 02:58:27', '2020-12-16 03:00:56', '2020-12-16 03:03:40', b'1', '2020-12-16 03:03:41', NULL);
INSERT INTO `stock_trans` VALUES (7625, 1, 7, 1, 163, 4914, 43, 49, 10, 13, 13, 19, '2020-12-16 02:59:17', '2020-12-16 02:59:18', '2020-12-16 03:02:10', b'1', '2020-12-16 03:03:31', NULL);
INSERT INTO `stock_trans` VALUES (7626, 0, 7, 1, 163, 4949, 15, 44, 8, 12, 12, 18, '2020-12-16 02:59:48', '2020-12-16 03:01:18', '2020-12-16 03:02:44', b'1', '2020-12-16 03:02:45', NULL);
INSERT INTO `stock_trans` VALUES (7627, 0, 7, 1, 174, 4950, 2, 19, 1, 11, 11, 17, '2020-12-16 03:03:41', '2020-12-16 03:05:50', '2020-12-16 03:08:02', b'1', '2020-12-16 03:08:03', NULL);
INSERT INTO `stock_trans` VALUES (7628, 0, 7, 1, 163, 4951, 9, 39, 5, 12, 12, 18, '2020-12-16 03:03:52', '2020-12-16 03:05:58', '2020-12-16 03:08:42', b'1', '2020-12-16 03:08:43', NULL);
INSERT INTO `stock_trans` VALUES (7629, 1, 7, 1, 174, 4910, 20, 18, 9, 13, 13, 15, '2020-12-16 03:04:04', '2020-12-16 03:04:05', '2020-12-16 03:06:43', b'1', '2020-12-16 03:07:51', NULL);
INSERT INTO `stock_trans` VALUES (7630, 0, 7, 1, 174, 4952, 1, 19, 1, 11, 11, 17, '2020-12-16 03:08:06', '2020-12-16 03:09:37', '2020-12-16 03:11:17', b'1', '2020-12-16 03:11:18', NULL);
INSERT INTO `stock_trans` VALUES (7631, 0, 7, 1, 163, 4953, 11, 39, 6, 12, 12, 18, '2020-12-16 03:08:44', '2020-12-16 03:10:19', '2020-12-16 03:12:44', b'1', '2020-12-16 03:12:45', NULL);
INSERT INTO `stock_trans` VALUES (7632, 1, 7, 1, 163, 4916, 43, 50, 10, 13, 13, 19, '2020-12-16 03:11:50', '2020-12-16 03:11:51', '2020-12-16 03:14:00', b'1', '2020-12-16 03:15:26', NULL);
INSERT INTO `stock_trans` VALUES (7633, 0, 7, 1, 163, 4954, 16, 39, 8, 12, 12, 18, '2020-12-16 03:12:46', '2020-12-16 03:15:03', '2020-12-16 03:17:35', b'1', '2020-12-16 03:17:36', NULL);
INSERT INTO `stock_trans` VALUES (7634, 1, 7, 1, 174, 4913, 20, 17, 9, 13, 13, 15, '2020-12-16 03:14:14', '2020-12-16 03:14:14', '2020-12-16 03:17:53', b'1', '2020-12-16 03:19:08', NULL);
INSERT INTO `stock_trans` VALUES (7635, 0, 7, 1, 174, 4955, 8, 19, 4, 11, 11, 17, '2020-12-16 03:18:24', '2020-12-16 03:21:04', '2020-12-16 03:23:13', b'1', '2020-12-16 03:23:14', NULL);
INSERT INTO `stock_trans` VALUES (7636, 1, 7, 1, 163, 4918, 42, 49, 10, 13, 13, 19, '2020-12-16 03:21:23', '2020-12-16 03:21:23', '2020-12-16 03:23:35', b'1', '2020-12-16 03:24:56', NULL);
INSERT INTO `stock_trans` VALUES (7637, 0, 7, 1, 174, 4956, 5, 20, 3, 11, 11, 17, '2020-12-16 03:23:15', '2020-12-16 03:25:09', '2020-12-16 03:27:47', b'1', '2020-12-16 03:27:48', NULL);
INSERT INTO `stock_trans` VALUES (7638, 0, 7, 1, 163, 4957, 15, 39, 8, 12, 12, 18, '2020-12-16 03:23:20', '2020-12-16 03:24:57', '2020-12-16 03:27:01', b'1', '2020-12-16 03:27:02', NULL);
INSERT INTO `stock_trans` VALUES (7639, 0, 7, 1, 163, 4958, 10, 39, 5, 12, 12, 18, '2020-12-16 03:27:09', '2020-12-16 03:29:27', '2020-12-16 03:34:38', b'1', '2020-12-16 03:34:39', NULL);
INSERT INTO `stock_trans` VALUES (7640, 1, 7, 1, 174, 4915, 21, 18, 9, 13, 13, 15, '2020-12-16 03:27:38', '2020-12-16 03:27:39', '2020-12-16 03:29:37', b'1', '2020-12-16 03:30:43', NULL);
INSERT INTO `stock_trans` VALUES (7641, 0, 7, 1, 174, 4959, 2, 20, 1, 11, 11, 17, '2020-12-16 03:27:48', '2020-12-16 03:29:55', '2020-12-16 03:32:19', b'1', '2020-12-16 03:32:20', NULL);
INSERT INTO `stock_trans` VALUES (7642, 0, 7, 1, 174, 4960, 1, 20, 1, 11, 11, 17, '2020-12-16 03:32:22', '2020-12-16 03:33:47', '2020-12-16 03:35:40', b'1', '2020-12-16 03:35:41', NULL);
INSERT INTO `stock_trans` VALUES (7643, 1, 7, 1, 163, 4919, 42, 50, 10, 13, 13, 19, '2020-12-16 03:32:57', '2020-12-16 03:32:57', '2020-12-16 03:35:01', b'1', '2020-12-16 03:36:15', NULL);
INSERT INTO `stock_trans` VALUES (7644, 0, 7, 1, 163, 4961, 12, 43, 6, 12, 12, 16, '2020-12-16 03:34:40', '2020-12-16 03:37:06', '2020-12-16 03:39:54', b'1', '2020-12-16 03:39:55', NULL);
INSERT INTO `stock_trans` VALUES (7645, 1, 7, 1, 174, 4917, 21, 17, 9, 13, 13, 15, '2020-12-16 03:35:44', '2020-12-16 03:35:45', '2020-12-16 03:38:33', b'1', '2020-12-16 03:39:31', NULL);
INSERT INTO `stock_trans` VALUES (7646, 0, 7, 1, 163, 4962, 16, 43, 8, 12, 12, 16, '2020-12-16 03:39:56', '2020-12-16 03:42:03', '2020-12-16 03:44:22', b'1', '2020-12-16 03:44:23', NULL);
INSERT INTO `stock_trans` VALUES (7647, 0, 7, 1, 174, 4963, 7, 20, 4, 11, 11, 17, '2020-12-16 03:42:52', '2020-12-16 03:44:52', '2020-12-16 03:47:10', b'1', '2020-12-16 03:47:11', NULL);
INSERT INTO `stock_trans` VALUES (7648, 1, 7, 1, 163, 4921, 42, 49, 10, 13, 13, 19, '2020-12-16 03:44:11', '2020-12-16 03:44:12', '2020-12-16 03:46:22', b'1', '2020-12-16 03:47:43', NULL);
INSERT INTO `stock_trans` VALUES (7649, 0, 7, 1, 163, 4964, 15, 43, 8, 12, 12, 16, '2020-12-16 03:45:11', '2020-12-16 03:46:44', '2020-12-16 03:48:44', b'1', '2020-12-16 03:48:45', NULL);
INSERT INTO `stock_trans` VALUES (7650, 1, 7, 1, 174, 4920, 21, 18, 9, 13, 13, 15, '2020-12-16 03:45:24', '2020-12-16 03:45:24', '2020-12-16 03:50:10', b'1', '2020-12-16 03:51:15', NULL);
INSERT INTO `stock_trans` VALUES (7651, 0, 7, 1, 174, 4965, 6, 20, 3, 11, 11, 17, '2020-12-16 03:47:12', '2020-12-16 03:49:38', '2020-12-16 03:51:36', b'1', '2020-12-16 03:51:37', NULL);
INSERT INTO `stock_trans` VALUES (7652, 0, 7, 1, 163, 4966, 9, 43, 5, 12, 12, 16, '2020-12-16 03:48:48', '2020-12-16 03:50:51', '2020-12-16 03:53:13', b'1', '2020-12-16 03:53:14', NULL);
INSERT INTO `stock_trans` VALUES (7653, 0, 7, 1, 174, 4967, 2, 25, 1, 11, 11, 17, '2020-12-16 03:51:38', '2020-12-16 03:53:45', '2020-12-16 03:56:21', b'1', '2020-12-16 03:56:22', NULL);
INSERT INTO `stock_trans` VALUES (7654, 0, 7, 1, 175, 4968, 4, 23, 2, 11, 11, 18, '2020-12-16 03:52:55', '2020-12-16 03:59:28', '2020-12-16 04:01:57', b'1', '2020-12-16 04:01:58', NULL);
INSERT INTO `stock_trans` VALUES (7655, 0, 7, 1, 163, 4969, 11, 43, 6, 12, 12, 16, '2020-12-16 03:53:15', '2020-12-16 03:55:04', '2020-12-16 03:56:49', b'1', '2020-12-16 03:56:50', NULL);
INSERT INTO `stock_trans` VALUES (7656, 1, 7, 1, 174, 4922, 21, 17, 9, 13, 13, 15, '2020-12-16 03:54:58', '2020-12-16 03:54:58', '2020-12-16 03:56:49', b'1', '2020-12-16 03:57:45', NULL);
INSERT INTO `stock_trans` VALUES (7657, 0, 7, 1, 174, 4970, 1, 25, 1, 11, 11, 17, '2020-12-16 03:56:25', '2020-12-16 04:02:27', '2020-12-16 04:04:49', b'1', '2020-12-16 04:04:50', NULL);
INSERT INTO `stock_trans` VALUES (7658, 0, 7, 1, 163, 4971, 16, 38, 8, 12, 12, 16, '2020-12-16 03:56:50', '2020-12-16 03:58:59', '2020-12-16 04:01:45', b'1', '2020-12-16 04:01:46', NULL);
INSERT INTO `stock_trans` VALUES (7659, 1, 7, 1, 163, 4923, 42, 50, 10, 13, 13, 19, '2020-12-16 03:57:26', '2020-12-16 03:57:27', '2020-12-16 04:00:19', b'1', '2020-12-16 04:01:32', NULL);
INSERT INTO `stock_trans` VALUES (7660, 1, 7, 1, 174, 4924, 21, 18, 9, 13, 13, 15, '2020-12-16 04:04:41', '2020-12-16 04:04:41', '2020-12-16 04:06:37', b'1', '2020-12-16 04:07:54', NULL);
INSERT INTO `stock_trans` VALUES (7661, 0, 7, 1, 174, 4972, 8, 25, 4, 11, 11, 17, '2020-12-16 04:06:20', '2020-12-16 04:08:39', '2020-12-16 04:11:03', b'1', '2020-12-16 04:11:04', NULL);
INSERT INTO `stock_trans` VALUES (7662, 1, 7, 1, 163, 4925, 42, 49, 10, 13, 13, 19, '2020-12-16 04:07:26', '2020-12-16 04:07:27', '2020-12-16 04:10:31', b'1', '2020-12-16 04:11:58', NULL);
INSERT INTO `stock_trans` VALUES (7663, 0, 7, 1, 174, 4973, 2, 25, 1, 11, 11, 17, '2020-12-16 04:11:04', '2020-12-16 04:13:11', '2020-12-16 04:15:12', b'1', '2020-12-16 04:15:13', NULL);
INSERT INTO `stock_trans` VALUES (7664, 1, 7, 1, 174, 4926, 22, 17, 9, 13, 13, 15, '2020-12-16 04:13:07', '2020-12-16 04:13:07', '2020-12-16 04:14:54', b'1', '2020-12-16 04:15:48', NULL);
INSERT INTO `stock_trans` VALUES (7665, 0, 7, 1, 174, 4974, 5, 25, 3, 11, 11, 17, '2020-12-16 04:15:15', '2020-12-16 04:16:43', '2020-12-16 04:18:08', b'1', '2020-12-16 04:18:09', NULL);
INSERT INTO `stock_trans` VALUES (7666, 0, 7, 1, 163, 4975, 15, 38, 8, 12, 12, 16, '2020-12-16 04:15:18', '2020-12-16 04:16:58', '2020-12-16 04:19:21', b'1', '2020-12-16 04:19:22', NULL);
INSERT INTO `stock_trans` VALUES (7667, 1, 7, 1, 163, 4927, 41, 50, 10, 13, 13, 19, '2020-12-16 04:17:10', '2020-12-16 04:17:11', '2020-12-16 04:19:05', b'1', '2020-12-16 04:20:17', NULL);
INSERT INTO `stock_trans` VALUES (7668, 0, 7, 1, 174, 4976, 1, 21, 1, 11, 11, 18, '2020-12-16 04:18:52', '2020-12-16 04:20:39', '2020-12-16 04:22:57', b'1', '2020-12-16 04:22:58', NULL);
INSERT INTO `stock_trans` VALUES (7669, 0, 7, 1, 163, 4977, 12, 38, 6, 12, 12, 16, '2020-12-16 04:19:23', '2020-12-16 04:21:24', '2020-12-16 04:23:27', b'1', '2020-12-16 04:23:28', NULL);
INSERT INTO `stock_trans` VALUES (7670, 0, 7, 1, 163, 4978, 10, 38, 5, 12, 12, 16, '2020-12-16 04:23:30', '2020-12-16 04:25:43', '2020-12-16 04:27:42', b'1', '2020-12-16 04:27:43', NULL);
INSERT INTO `stock_trans` VALUES (7671, 1, 7, 1, 174, 4928, 22, 18, 9, 13, 13, 15, '2020-12-16 04:23:35', '2020-12-16 04:23:35', '2020-12-16 04:25:25', b'1', '2020-12-16 04:26:27', NULL);
INSERT INTO `stock_trans` VALUES (7672, 1, 7, 1, 163, 4930, 41, 49, 10, 13, 13, 19, '2020-12-16 04:28:34', '2020-12-16 04:28:35', '2020-12-16 04:30:40', b'1', '2020-12-16 04:31:55', NULL);
INSERT INTO `stock_trans` VALUES (7673, 0, 7, 1, 174, 4979, 7, 21, 4, 11, 11, 18, '2020-12-16 04:31:01', '2020-12-16 04:32:56', '2020-12-16 04:35:40', b'1', '2020-12-16 04:35:41', NULL);
INSERT INTO `stock_trans` VALUES (7674, 1, 7, 1, 174, 4931, 22, 17, 9, 13, 13, 15, '2020-12-16 04:32:43', '2020-12-16 04:32:43', '2020-12-16 04:34:32', b'1', '2020-12-16 04:35:27', NULL);
INSERT INTO `stock_trans` VALUES (7675, 0, 7, 1, 163, 4980, 16, 38, 8, 12, 12, 16, '2020-12-16 04:33:01', '2020-12-16 04:35:28', '2020-12-16 04:37:12', b'1', '2020-12-16 04:37:13', NULL);
INSERT INTO `stock_trans` VALUES (7676, 0, 7, 1, 174, 4981, 2, 21, 1, 11, 11, 18, '2020-12-16 04:35:42', '2020-12-16 04:37:42', '2020-12-16 04:39:46', b'1', '2020-12-16 04:39:48', NULL);
INSERT INTO `stock_trans` VALUES (7677, 0, 7, 1, 174, 4982, 6, 21, 3, 11, 11, 18, '2020-12-16 04:39:49', '2020-12-16 04:42:08', '2020-12-16 04:44:20', b'1', '2020-12-16 04:44:21', NULL);
INSERT INTO `stock_trans` VALUES (7678, 1, 7, 1, 174, 4933, 22, 18, 9, 13, 13, 15, '2020-12-16 04:43:04', '2020-12-16 04:43:04', '2020-12-16 04:44:57', b'1', '2020-12-16 04:45:58', NULL);
INSERT INTO `stock_trans` VALUES (7679, 0, 7, 1, 174, 4983, 1, 21, 1, 11, 11, 18, '2020-12-16 04:44:23', '2020-12-16 04:45:47', '2020-12-16 04:47:05', b'1', '2020-12-16 04:47:06', NULL);
INSERT INTO `stock_trans` VALUES (7680, 0, 7, 1, 163, 4984, 15, 42, 8, 12, 12, 16, '2020-12-16 04:44:39', '2020-12-16 04:46:21', '2020-12-16 04:48:33', b'1', '2020-12-16 04:48:34', NULL);
INSERT INTO `stock_trans` VALUES (7681, 0, 7, 1, 163, 4985, 9, 42, 5, 12, 12, 16, '2020-12-16 04:48:34', '2020-12-16 04:50:28', '2020-12-16 04:53:05', b'1', '2020-12-16 04:53:06', NULL);
INSERT INTO `stock_trans` VALUES (7682, 1, 7, 1, 163, 4932, 41, 50, 10, 13, 13, 19, '2020-12-16 04:50:14', '2020-12-16 04:50:14', '2020-12-16 04:52:08', b'1', '2020-12-16 04:53:18', NULL);
INSERT INTO `stock_trans` VALUES (7683, 0, 7, 1, 163, 4986, 11, 42, 6, 12, 12, 16, '2020-12-16 04:53:08', '2020-12-16 04:54:52', '2020-12-16 04:57:07', b'1', '2020-12-16 04:57:08', NULL);
INSERT INTO `stock_trans` VALUES (7684, 1, 7, 1, 174, 4934, 22, 17, 9, 13, 13, 15, '2020-12-16 04:54:09', '2020-12-16 04:54:10', '2020-12-16 04:55:55', b'1', '2020-12-16 04:57:06', NULL);
INSERT INTO `stock_trans` VALUES (7685, 1, 7, 1, 163, 4935, 41, 49, 10, 13, 13, 19, '2020-12-16 04:58:59', '2020-12-16 04:58:59', '2020-12-16 05:01:13', b'1', '2020-12-16 05:02:28', NULL);
INSERT INTO `stock_trans` VALUES (7686, 1, 7, 1, 174, 4936, 24, 18, 9, 13, 13, 15, '2020-12-16 05:05:07', '2020-12-16 05:05:07', '2020-12-16 05:06:46', b'1', '2020-12-16 05:07:40', NULL);
INSERT INTO `stock_trans` VALUES (7687, 0, 7, 1, 163, 4987, 16, 42, 8, 12, 12, 16, '2020-12-16 05:05:51', '2020-12-16 05:07:55', '2020-12-16 05:09:49', b'1', '2020-12-16 05:09:50', NULL);
INSERT INTO `stock_trans` VALUES (7688, 1, 7, 1, 163, 4937, 41, 50, 10, 13, 13, 19, '2020-12-16 05:09:02', '2020-12-16 05:09:03', '2020-12-16 05:11:01', b'1', '2020-12-16 05:12:30', NULL);
INSERT INTO `stock_trans` VALUES (7689, 1, 7, 1, 174, 4939, 24, 17, 9, 13, 13, 15, '2020-12-16 05:17:27', '2020-12-16 05:17:28', '2020-12-16 05:19:08', b'1', '2020-12-16 05:20:04', NULL);
INSERT INTO `stock_trans` VALUES (7690, 1, 7, 1, 163, 4940, 44, 49, 10, 13, 13, 19, '2020-12-16 05:18:52', '2020-12-16 05:18:53', '2020-12-16 05:22:48', b'1', '2020-12-16 05:24:11', NULL);
INSERT INTO `stock_trans` VALUES (7691, 0, 7, 1, 174, 4988, 8, 22, 4, 11, 11, 18, '2020-12-16 05:19:22', '2020-12-16 05:21:57', '2020-12-16 05:24:56', b'1', '2020-12-16 05:24:57', NULL);
INSERT INTO `stock_trans` VALUES (7692, 0, 7, 1, 174, 4989, 5, 22, 3, 11, 11, 18, '2020-12-16 05:24:58', '2020-12-16 05:26:36', '2020-12-16 05:28:57', b'1', '2020-12-16 05:28:58', NULL);
INSERT INTO `stock_trans` VALUES (7693, 0, 7, 1, 163, 4990, 15, 42, 8, 12, 12, 16, '2020-12-16 05:27:23', '2020-12-16 05:28:49', '2020-12-16 05:30:07', b'1', '2020-12-16 05:30:08', NULL);
INSERT INTO `stock_trans` VALUES (7694, 1, 7, 1, 174, 4941, 24, 18, 9, 13, 13, 15, '2020-12-16 05:27:58', '2020-12-16 05:27:59', '2020-12-16 05:29:38', b'1', '2020-12-16 05:30:31', NULL);
INSERT INTO `stock_trans` VALUES (7695, 0, 7, 1, 164, 4991, 13, 40, 7, 12, 12, 17, '2020-12-16 05:28:50', '2020-12-16 05:33:10', '2020-12-16 05:35:16', b'1', '2020-12-16 05:35:17', NULL);
INSERT INTO `stock_trans` VALUES (7696, 0, 7, 1, 174, 4992, 2, 22, 1, 11, 11, 18, '2020-12-16 05:28:59', '2020-12-16 05:30:58', '2020-12-16 05:32:58', b'1', '2020-12-16 05:32:59', NULL);
INSERT INTO `stock_trans` VALUES (7697, 1, 7, 1, 163, 4942, 44, 50, 10, 13, 13, 19, '2020-12-16 05:29:23', '2020-12-16 05:29:25', '2020-12-16 05:33:08', b'1', '2020-12-16 05:34:27', NULL);
INSERT INTO `stock_trans` VALUES (7698, 0, 7, 1, 163, 4993, 10, 41, 5, 12, 12, 16, '2020-12-16 05:31:33', '2020-12-16 05:36:46', '2020-12-16 05:39:36', b'1', '2020-12-16 05:39:37', NULL);
INSERT INTO `stock_trans` VALUES (7699, 0, 7, 1, 174, 4994, 1, 22, 1, 11, 11, 18, '2020-12-16 05:35:15', '2020-12-16 05:36:40', '2020-12-16 05:38:26', b'1', '2020-12-16 05:38:27', NULL);
INSERT INTO `stock_trans` VALUES (7700, 1, 7, 1, 174, 4943, 24, 17, 9, 13, 13, 15, '2020-12-16 05:35:36', '2020-12-16 05:35:37', '2020-12-16 05:37:36', b'1', '2020-12-16 05:38:33', NULL);
INSERT INTO `stock_trans` VALUES (7701, 1, 7, 1, 163, 4944, 44, 49, 10, 13, 13, 19, '2020-12-16 05:39:34', '2020-12-16 05:39:35', '2020-12-16 05:41:57', b'1', '2020-12-16 05:43:19', NULL);
INSERT INTO `stock_trans` VALUES (7702, 0, 7, 1, 163, 4995, 16, 41, 8, 12, 12, 16, '2020-12-16 05:39:38', '2020-12-16 05:41:51', '2020-12-16 05:44:16', b'1', '2020-12-16 05:44:17', NULL);
INSERT INTO `stock_trans` VALUES (7703, 0, 7, 1, 163, 4996, 12, 41, 6, 12, 12, 16, '2020-12-16 05:44:18', '2020-12-16 05:46:28', '2020-12-16 05:48:51', b'1', '2020-12-16 05:48:52', NULL);
INSERT INTO `stock_trans` VALUES (7704, 0, 7, 1, 174, 4997, 7, 22, 4, 11, 11, 18, '2020-12-16 05:44:38', '2020-12-16 05:46:32', '2020-12-16 05:48:21', b'1', '2020-12-16 05:48:22', NULL);
INSERT INTO `stock_trans` VALUES (7705, 1, 7, 1, 174, 4945, 24, 18, 9, 13, 13, 15, '2020-12-16 05:47:46', '2020-12-16 05:47:47', '2020-12-16 05:49:28', b'1', '2020-12-16 05:50:54', NULL);
INSERT INTO `stock_trans` VALUES (7706, 0, 7, 1, 174, 4998, 6, 26, 3, 11, 11, 18, '2020-12-16 05:48:24', '2020-12-16 05:50:45', '2020-12-16 05:53:18', b'1', '2020-12-16 05:53:19', NULL);
INSERT INTO `stock_trans` VALUES (7707, 1, 7, 1, 163, 4946, 44, 50, 10, 13, 13, 19, '2020-12-16 05:50:26', '2020-12-16 05:50:27', '2020-12-16 05:53:44', b'1', '2020-12-16 05:55:00', NULL);
INSERT INTO `stock_trans` VALUES (7708, 0, 7, 1, 174, 4999, 2, 26, 1, 11, 11, 18, '2020-12-16 05:53:19', '2020-12-16 05:55:29', '2020-12-16 05:58:01', b'1', '2020-12-16 05:58:02', NULL);
INSERT INTO `stock_trans` VALUES (7709, 0, 7, 1, 175, 5000, 3, 23, 2, 11, 11, 17, '2020-12-16 05:54:10', '2020-12-16 06:00:08', '2020-12-16 06:02:06', b'1', '2020-12-16 06:02:07', NULL);
INSERT INTO `stock_trans` VALUES (7710, 1, 7, 1, 174, 4947, 19, 17, 9, 13, 13, 15, '2020-12-16 05:57:17', '2020-12-16 05:57:17', '2020-12-16 05:59:17', b'1', '2020-12-16 06:00:20', NULL);
INSERT INTO `stock_trans` VALUES (7711, 0, 7, 1, 163, 5001, 15, 41, 8, 12, 12, 16, '2020-12-16 05:59:18', '2020-12-16 06:01:08', '2020-12-16 06:02:49', b'1', '2020-12-16 06:02:50', NULL);
INSERT INTO `stock_trans` VALUES (7712, 0, 7, 1, 174, 5002, 1, 26, 1, 11, 11, 18, '2020-12-16 05:59:32', '2020-12-16 06:02:59', '2020-12-16 06:05:11', b'1', '2020-12-16 06:05:12', NULL);
INSERT INTO `stock_trans` VALUES (7713, 1, 7, 1, 163, 4949, 44, 49, 10, 13, 13, 19, '2020-12-16 05:59:45', '2020-12-16 05:59:46', '2020-12-16 06:03:15', b'1', '2020-12-16 06:04:38', NULL);
INSERT INTO `stock_trans` VALUES (7714, 0, 7, 1, 163, 5003, 11, 41, 6, 12, 12, 16, '2020-12-16 06:02:50', '2020-12-16 06:04:33', '2020-12-16 06:06:09', b'1', '2020-12-16 06:06:10', NULL);
INSERT INTO `stock_trans` VALUES (7715, 0, 7, 1, 163, 5004, 9, 44, 5, 12, 12, 16, '2020-12-16 06:06:11', '2020-12-16 06:08:04', '2020-12-16 06:10:57', b'1', '2020-12-16 06:10:58', NULL);
INSERT INTO `stock_trans` VALUES (7716, 1, 7, 1, 174, 4948, 19, 18, 9, 13, 13, 15, '2020-12-16 06:08:12', '2020-12-16 06:08:12', '2020-12-16 06:10:19', b'1', '2020-12-16 06:11:28', NULL);
INSERT INTO `stock_trans` VALUES (7717, 0, 7, 1, 174, 5005, 8, 26, 4, 11, 11, 18, '2020-12-16 06:08:53', '2020-12-16 06:11:11', '2020-12-16 06:13:19', b'1', '2020-12-16 06:13:20', NULL);
INSERT INTO `stock_trans` VALUES (7718, 1, 7, 1, 163, 4951, 39, 50, 10, 13, 13, 19, '2020-12-16 06:11:54', '2020-12-16 06:11:54', '2020-12-16 06:14:16', b'1', '2020-12-16 06:15:16', NULL);
INSERT INTO `stock_trans` VALUES (7719, 0, 7, 1, 174, 5006, 2, 26, 1, 11, 11, 18, '2020-12-16 06:13:20', '2020-12-16 06:15:29', '2020-12-16 06:17:09', b'1', '2020-12-16 06:17:10', NULL);
INSERT INTO `stock_trans` VALUES (7720, 1, 7, 1, 174, 4950, 19, 17, 9, 13, 13, 15, '2020-12-16 06:16:21', '2020-12-16 06:16:21', '2020-12-16 06:18:45', b'1', '2020-12-16 06:19:48', NULL);
INSERT INTO `stock_trans` VALUES (7721, 0, 7, 1, 174, 5007, 5, 24, 3, 11, 11, 17, '2020-12-16 06:17:11', '2020-12-16 06:19:07', '2020-12-16 06:21:31', b'1', '2020-12-16 06:21:32', NULL);
INSERT INTO `stock_trans` VALUES (7722, 0, 7, 1, 174, 5008, 1, 24, 1, 11, 11, 17, '2020-12-16 06:21:45', '2020-12-16 06:23:22', '2020-12-16 06:25:38', b'1', '2020-12-16 06:25:39', NULL);
INSERT INTO `stock_trans` VALUES (7723, 1, 7, 1, 163, 4953, 39, 49, 10, 13, 13, 19, '2020-12-16 06:22:17', '2020-12-16 06:22:18', '2020-12-16 06:24:10', b'1', '2020-12-16 06:25:21', NULL);
INSERT INTO `stock_trans` VALUES (7724, 0, 7, 1, 163, 5009, 16, 44, 8, 12, 12, 16, '2020-12-16 06:24:49', '2020-12-16 06:26:50', '2020-12-16 06:29:06', b'1', '2020-12-16 06:29:07', NULL);
INSERT INTO `stock_trans` VALUES (7725, 0, 7, 1, 174, 5010, 7, 24, 4, 11, 11, 17, '2020-12-16 06:30:59', '2020-12-16 06:32:43', '2020-12-16 06:35:00', b'1', '2020-12-16 06:35:01', NULL);
INSERT INTO `stock_trans` VALUES (7726, 1, 7, 1, 174, 4952, 19, 18, 9, 13, 13, 15, '2020-12-16 06:32:09', '2020-12-16 06:32:09', '2020-12-16 06:34:18', b'1', '2020-12-16 06:35:29', NULL);
INSERT INTO `stock_trans` VALUES (7727, 0, 7, 1, 163, 5011, 10, 44, 5, 12, 12, 16, '2020-12-16 06:34:21', '2020-12-16 06:36:58', '2020-12-16 06:39:42', b'1', '2020-12-16 06:39:43', NULL);
INSERT INTO `stock_trans` VALUES (7728, 1, 7, 1, 163, 4954, 39, 50, 10, 13, 13, 19, '2020-12-16 06:34:23', '2020-12-16 06:34:24', '2020-12-16 06:37:50', b'1', '2020-12-16 06:38:55', NULL);
INSERT INTO `stock_trans` VALUES (7729, 0, 7, 1, 174, 5012, 6, 24, 3, 11, 11, 17, '2020-12-16 06:35:01', '2020-12-16 06:37:16', '2020-12-16 06:39:23', b'1', '2020-12-16 06:39:24', NULL);
INSERT INTO `stock_trans` VALUES (7730, 0, 7, 1, 174, 5013, 2, 24, 1, 11, 11, 17, '2020-12-16 06:39:25', '2020-12-16 06:41:32', '2020-12-16 06:43:08', b'1', '2020-12-16 06:43:09', NULL);
INSERT INTO `stock_trans` VALUES (7731, 0, 7, 1, 163, 5014, 15, 44, 8, 12, 12, 16, '2020-12-16 06:39:43', '2020-12-16 06:41:10', '2020-12-16 06:43:00', b'1', '2020-12-16 06:43:01', NULL);
INSERT INTO `stock_trans` VALUES (7732, 1, 7, 1, 174, 4955, 19, 17, 9, 13, 13, 15, '2020-12-16 06:40:45', '2020-12-16 06:40:45', '2020-12-16 06:42:44', b'1', '2020-12-16 06:43:59', NULL);
INSERT INTO `stock_trans` VALUES (7733, 0, 7, 1, 163, 5015, 12, 44, 6, 12, 12, 16, '2020-12-16 06:43:02', '2020-12-16 06:45:24', '2020-12-16 06:47:18', b'1', '2020-12-16 06:47:19', NULL);
INSERT INTO `stock_trans` VALUES (7734, 0, 7, 1, 174, 5016, 1, 27, 1, 11, 11, 17, '2020-12-16 06:43:41', '2020-12-16 06:45:16', '2020-12-16 06:47:55', b'1', '2020-12-16 06:47:56', NULL);
INSERT INTO `stock_trans` VALUES (7735, 1, 7, 1, 163, 4957, 39, 49, 10, 13, 13, 19, '2020-12-16 06:44:16', '2020-12-16 06:44:17', '2020-12-16 06:46:42', b'1', '2020-12-16 06:47:52', NULL);
INSERT INTO `stock_trans` VALUES (7736, 1, 7, 1, 174, 4956, 20, 18, 9, 13, 13, 15, '2020-12-16 06:48:41', '2020-12-16 06:48:42', '2020-12-16 07:01:30', b'1', '2020-12-16 07:02:36', NULL);
INSERT INTO `stock_trans` VALUES (7737, 0, 7, 1, 163, 5017, 16, 37, 8, 12, 12, 16, '2020-12-16 06:50:43', '2020-12-16 06:52:47', '2020-12-16 06:55:34', b'1', '2020-12-16 06:55:35', NULL);
INSERT INTO `stock_trans` VALUES (7738, 1, 7, 1, 163, 4958, 39, 50, 10, 13, 13, 19, '2020-12-16 06:54:18', '2020-12-16 06:54:19', '2020-12-16 07:04:52', b'1', '2020-12-16 07:06:20', NULL);
INSERT INTO `stock_trans` VALUES (7739, 0, 7, 1, 174, 5018, 8, 27, 4, 11, 11, 17, '2020-12-16 06:54:41', '2020-12-16 06:56:53', '2020-12-16 06:59:23', b'1', '2020-12-16 06:59:24', NULL);
INSERT INTO `stock_trans` VALUES (7740, 0, 7, 1, 174, 5019, 5, 27, 3, 11, 11, 17, '2020-12-16 06:59:24', '2020-12-16 07:00:41', '2020-12-16 07:02:29', b'1', '2020-12-16 07:02:30', NULL);
INSERT INTO `stock_trans` VALUES (7741, 0, 7, 1, 174, 5020, 2, 27, 1, 11, 11, 17, '2020-12-16 07:02:30', '2020-12-16 07:04:46', '2020-12-16 07:06:53', b'1', '2020-12-16 07:06:54', NULL);
INSERT INTO `stock_trans` VALUES (7742, 1, 7, 1, 174, 4959, 20, 17, 9, 13, 13, 15, '2020-12-16 07:02:38', '2020-12-16 07:03:09', '2020-12-16 07:08:45', b'1', '2020-12-16 07:09:48', NULL);
INSERT INTO `stock_trans` VALUES (7743, 1, 7, 1, 163, 4961, 43, 49, 10, 13, 13, 19, '2020-12-16 07:06:21', '2020-12-16 07:06:31', '2020-12-16 07:12:33', b'1', '2020-12-16 07:13:52', NULL);
INSERT INTO `stock_trans` VALUES (7744, 0, 7, 1, 174, 5021, 1, 27, 1, 11, 11, 17, '2020-12-16 07:07:21', '2020-12-16 07:09:02', '2020-12-16 07:10:41', b'1', '2020-12-16 07:10:42', NULL);
INSERT INTO `stock_trans` VALUES (7745, 1, 7, 1, 174, 4960, 20, 18, 9, 13, 13, 15, '2020-12-16 07:16:36', '2020-12-16 07:16:37', '2020-12-16 07:18:39', b'1', '2020-12-16 07:19:44', NULL);
INSERT INTO `stock_trans` VALUES (7746, 1, 7, 1, 163, 4962, 43, 50, 10, 13, 13, 19, '2020-12-16 07:16:42', '2020-12-16 07:16:43', '2020-12-16 07:22:27', b'1', '2020-12-16 07:23:41', NULL);
INSERT INTO `stock_trans` VALUES (7747, 0, 7, 1, 174, 5022, 7, 19, 4, 11, 11, 18, '2020-12-16 07:18:02', '2020-12-16 07:19:55', '2020-12-16 07:22:55', b'1', '2020-12-16 07:22:56', NULL);
INSERT INTO `stock_trans` VALUES (7748, 0, 7, 1, 174, 5023, 6, 19, 3, 11, 11, 18, '2020-12-16 07:22:57', '2020-12-16 07:25:25', '2020-12-16 07:28:12', b'1', '2020-12-16 07:28:13', NULL);
INSERT INTO `stock_trans` VALUES (7749, 1, 7, 1, 174, 4963, 20, 17, 9, 13, 13, 15, '2020-12-16 07:24:59', '2020-12-16 07:24:59', '2020-12-16 07:27:13', b'1', '2020-12-16 07:28:13', NULL);
INSERT INTO `stock_trans` VALUES (7750, 1, 7, 1, 163, 4964, 43, 49, 10, 13, 13, 19, '2020-12-16 07:26:21', '2020-12-16 07:26:21', '2020-12-16 07:31:01', b'1', '2020-12-16 07:32:21', NULL);
INSERT INTO `stock_trans` VALUES (7751, 0, 7, 1, 174, 5024, 2, 19, 1, 11, 11, 18, '2020-12-16 07:28:13', '2020-12-16 07:30:22', '2020-12-16 07:32:30', b'1', '2020-12-16 07:32:31', NULL);
INSERT INTO `stock_trans` VALUES (7752, 0, 7, 1, 174, 5025, 1, 19, 1, 11, 11, 18, '2020-12-16 07:32:33', '2020-12-16 07:34:02', '2020-12-16 07:35:48', b'1', '2020-12-16 07:35:49', NULL);
INSERT INTO `stock_trans` VALUES (7753, 1, 7, 1, 163, 4966, 43, 50, 10, 13, 13, 19, '2020-12-16 07:34:10', '2020-12-16 07:34:10', '2020-12-16 07:36:16', b'1', '2020-12-16 07:37:29', NULL);
INSERT INTO `stock_trans` VALUES (7754, 0, 7, 1, 163, 5026, 9, 37, 5, 12, 12, 16, '2020-12-16 07:34:18', '2020-12-16 07:35:56', '2020-12-16 07:38:14', b'1', '2020-12-16 07:38:15', NULL);
INSERT INTO `stock_trans` VALUES (7755, 1, 7, 1, 174, 4965, 20, 18, 9, 13, 13, 15, '2020-12-16 07:36:47', '2020-12-16 07:36:48', '2020-12-16 07:40:06', b'1', '2020-12-16 07:41:14', NULL);
INSERT INTO `stock_trans` VALUES (7756, 0, 7, 1, 163, 5027, 15, 37, 8, 12, 12, 16, '2020-12-16 07:38:16', '2020-12-16 07:40:02', '2020-12-16 07:42:15', b'1', '2020-12-16 07:42:16', NULL);
INSERT INTO `stock_trans` VALUES (7757, 0, 7, 1, 174, 5028, 8, 19, 4, 11, 11, 18, '2020-12-16 07:42:35', '2020-12-16 07:45:18', '2020-12-16 07:47:26', b'1', '2020-12-16 07:47:27', NULL);
INSERT INTO `stock_trans` VALUES (7758, 1, 7, 1, 163, 4969, 43, 49, 10, 13, 13, 19, '2020-12-16 07:43:06', '2020-12-16 07:43:07', '2020-12-16 07:45:24', b'1', '2020-12-16 07:46:43', NULL);
INSERT INTO `stock_trans` VALUES (7759, 0, 7, 1, 163, 5029, 11, 37, 6, 12, 12, 16, '2020-12-16 07:43:11', '2020-12-16 07:44:44', '2020-12-16 07:46:26', b'1', '2020-12-16 07:46:27', NULL);
INSERT INTO `stock_trans` VALUES (7760, 1, 7, 1, 174, 4967, 25, 17, 9, 13, 13, 15, '2020-12-16 07:44:27', '2020-12-16 07:44:28', '2020-12-16 07:48:40', b'1', '2020-12-16 07:49:42', NULL);
INSERT INTO `stock_trans` VALUES (7761, 0, 7, 1, 174, 5030, 2, 20, 1, 11, 11, 18, '2020-12-16 07:47:28', '2020-12-16 07:49:39', '2020-12-16 07:52:12', b'1', '2020-12-16 07:52:13', NULL);
INSERT INTO `stock_trans` VALUES (7762, 1, 7, 1, 163, 4971, 38, 50, 10, 13, 13, 19, '2020-12-16 07:51:10', '2020-12-16 07:51:11', '2020-12-16 07:52:49', b'1', '2020-12-16 07:53:50', NULL);
INSERT INTO `stock_trans` VALUES (7763, 0, 7, 1, 174, 5031, 5, 20, 3, 11, 11, 18, '2020-12-16 07:52:15', '2020-12-16 07:54:05', '2020-12-16 07:56:31', b'1', '2020-12-16 07:56:32', NULL);
INSERT INTO `stock_trans` VALUES (7764, 1, 7, 1, 174, 4970, 25, 18, 9, 13, 13, 15, '2020-12-16 07:52:54', '2020-12-16 07:52:55', '2020-12-16 07:55:41', b'1', '2020-12-16 07:56:32', NULL);
INSERT INTO `stock_trans` VALUES (7765, 1, 7, 1, 174, 4972, 25, 17, 9, 13, 13, 15, '2020-12-16 08:01:16', '2020-12-16 08:01:17', '2020-12-16 08:02:55', b'1', '2020-12-16 08:03:56', NULL);
INSERT INTO `stock_trans` VALUES (7766, 1, 7, 1, 163, 4975, 38, 49, 10, 13, 13, 19, '2020-12-16 08:01:17', '2020-12-16 08:01:17', '2020-12-16 08:06:03', b'1', '2020-12-16 08:07:11', NULL);
INSERT INTO `stock_trans` VALUES (7767, 0, 7, 1, 174, 5032, 1, 20, 1, 11, 11, 18, '2020-12-16 08:02:54', '2020-12-16 08:04:18', '2020-12-16 08:06:13', b'1', '2020-12-16 08:06:14', NULL);
INSERT INTO `stock_trans` VALUES (7768, 0, 7, 1, 163, 5033, 16, 37, 8, 12, 12, 16, '2020-12-16 08:03:20', '2020-12-16 08:05:47', '2020-12-16 08:07:33', b'1', '2020-12-16 08:07:34', NULL);
INSERT INTO `stock_trans` VALUES (7769, 1, 7, 1, 163, 4977, 38, 50, 10, 13, 13, 19, '2020-12-16 08:09:03', '2020-12-16 08:09:03', '2020-12-16 08:10:39', b'1', '2020-12-16 08:11:39', NULL);
INSERT INTO `stock_trans` VALUES (7770, 1, 7, 1, 174, 4973, 25, 18, 9, 13, 13, 15, '2020-12-16 08:09:14', '2020-12-16 08:09:14', '2020-12-16 08:13:28', b'1', '2020-12-16 08:14:18', NULL);
INSERT INTO `stock_trans` VALUES (7771, 1, 7, 1, 163, 4978, 38, 49, 10, 13, 13, 19, '2020-12-16 08:16:46', '2020-12-16 08:16:46', '2020-12-16 08:18:35', b'1', '2020-12-16 08:19:43', NULL);
INSERT INTO `stock_trans` VALUES (7772, 1, 7, 1, 174, 4974, 25, 17, 9, NULL, NULL, 15, '2020-12-16 08:16:56', '2020-12-16 08:16:56', NULL, b'1', '2020-12-16 08:18:09', NULL);
INSERT INTO `stock_trans` VALUES (7773, 1, 7, 1, 174, 4974, 25, 17, 9, 13, 13, 15, '2020-12-16 08:18:14', '2020-12-16 08:18:14', '2020-12-16 08:21:44', b'1', '2020-12-16 08:22:55', NULL);
INSERT INTO `stock_trans` VALUES (7774, 0, 7, 1, 175, 5034, 4, 23, 2, 11, 11, 18, '2020-12-16 08:20:21', '2020-12-16 08:22:36', '2020-12-16 08:24:30', b'1', '2020-12-16 08:24:31', NULL);
INSERT INTO `stock_trans` VALUES (7775, 0, 7, 1, 174, 5035, 7, 20, 4, 11, 11, 17, '2020-12-16 08:20:48', '2020-12-16 08:25:40', '2020-12-16 08:27:56', b'1', '2020-12-16 08:27:57', NULL);
INSERT INTO `stock_trans` VALUES (7776, 0, 7, 1, 163, 5036, 10, 43, 5, 12, 12, 16, '2020-12-16 08:25:27', '2020-12-16 08:27:44', '2020-12-16 08:30:44', b'1', '2020-12-16 08:30:45', NULL);
INSERT INTO `stock_trans` VALUES (7777, 1, 7, 1, 174, 4976, 21, 18, 9, 13, 13, 15, '2020-12-16 08:25:32', '2020-12-16 08:25:33', '2020-12-16 08:27:32', b'1', '2020-12-16 08:28:39', NULL);
INSERT INTO `stock_trans` VALUES (7778, 0, 7, 1, 174, 5037, 2, 20, 1, 11, 11, 17, '2020-12-16 08:27:58', '2020-12-16 08:30:05', '2020-12-16 08:31:37', b'1', '2020-12-16 08:31:38', NULL);
INSERT INTO `stock_trans` VALUES (7779, 1, 7, 1, 163, 4980, 38, 50, 10, 13, 13, 19, '2020-12-16 08:28:31', '2020-12-16 08:28:31', '2020-12-16 08:30:47', b'1', '2020-12-16 08:32:12', NULL);
INSERT INTO `stock_trans` VALUES (7780, 0, 7, 1, 163, 5038, 15, 43, 8, 12, 12, 16, '2020-12-16 08:30:46', '2020-12-16 08:32:11', '2020-12-16 08:34:17', b'1', '2020-12-16 08:34:18', NULL);
INSERT INTO `stock_trans` VALUES (7781, 0, 7, 1, 174, 5039, 6, 25, 3, 11, 11, 17, '2020-12-16 08:31:39', '2020-12-16 08:34:11', '2020-12-16 08:36:47', b'1', '2020-12-16 08:36:48', NULL);
INSERT INTO `stock_trans` VALUES (7782, 1, 7, 1, 174, 4979, 21, 17, 9, 13, 13, 15, '2020-12-16 08:33:44', '2020-12-16 08:33:44', '2020-12-16 08:35:32', b'1', '2020-12-16 08:36:30', NULL);
INSERT INTO `stock_trans` VALUES (7783, 0, 7, 1, 163, 5040, 12, 43, 6, 12, 12, 16, '2020-12-16 08:34:20', '2020-12-16 08:36:39', '2020-12-16 08:39:00', b'1', '2020-12-16 08:39:01', NULL);
INSERT INTO `stock_trans` VALUES (7784, 0, 7, 1, 163, 5041, 16, 43, 8, 12, 12, 16, '2020-12-16 08:39:04', '2020-12-16 08:41:07', '2020-12-16 08:42:57', b'1', '2020-12-16 08:42:58', NULL);
INSERT INTO `stock_trans` VALUES (7785, 1, 7, 1, 163, 4984, 42, 49, 10, 13, 13, 19, '2020-12-16 08:40:46', '2020-12-16 08:40:46', '2020-12-16 08:42:57', b'1', '2020-12-16 08:44:14', NULL);
INSERT INTO `stock_trans` VALUES (7786, 1, 7, 1, 174, 4981, 21, 18, 9, 13, 13, 15, '2020-12-16 08:43:02', '2020-12-16 08:43:02', '2020-12-16 08:46:39', b'1', '2020-12-16 08:47:43', NULL);
INSERT INTO `stock_trans` VALUES (7787, 0, 7, 1, 174, 5042, 1, 25, 1, 11, 11, 17, '2020-12-16 08:48:23', '2020-12-16 08:50:02', '2020-12-16 08:52:28', b'1', '2020-12-16 08:52:29', NULL);
INSERT INTO `stock_trans` VALUES (7788, 1, 7, 1, 163, 4985, 42, 50, 10, 13, 13, 19, '2020-12-16 08:49:14', '2020-12-16 08:49:15', '2020-12-16 08:51:15', b'1', '2020-12-16 08:52:27', NULL);
INSERT INTO `stock_trans` VALUES (7789, 0, 7, 1, 163, 5043, 11, 43, 6, 12, 12, 16, '2020-12-16 08:49:33', '2020-12-16 08:51:21', '2020-12-16 08:53:05', b'1', '2020-12-16 08:53:06', NULL);
INSERT INTO `stock_trans` VALUES (7790, 0, 7, 1, 164, 5044, 14, 40, 7, 12, 12, 18, '2020-12-16 08:50:37', '2020-12-16 08:57:16', '2020-12-16 08:59:21', b'1', '2020-12-16 08:59:22', NULL);
INSERT INTO `stock_trans` VALUES (7791, 1, 7, 1, 174, 4982, 21, 17, 9, 13, 13, 15, '2020-12-16 08:51:11', '2020-12-16 08:51:11', '2020-12-16 08:54:46', b'1', '2020-12-16 08:55:43', NULL);
INSERT INTO `stock_trans` VALUES (7792, 0, 7, 1, 163, 5045, 15, 39, 8, 12, 12, 16, '2020-12-16 08:53:08', '2020-12-16 08:59:58', '2020-12-16 09:02:30', b'1', '2020-12-16 09:02:31', NULL);
INSERT INTO `stock_trans` VALUES (7793, 1, 7, 1, 163, 4986, 42, 49, 10, 13, 13, 19, '2020-12-16 08:59:07', '2020-12-16 08:59:07', '2020-12-16 09:01:56', b'1', '2020-12-16 09:03:14', NULL);
INSERT INTO `stock_trans` VALUES (7794, 1, 7, 1, 174, 4983, 21, 18, 9, 13, 13, 15, '2020-12-16 08:59:33', '2020-12-16 08:59:33', '2020-12-16 09:05:38', b'1', '2020-12-16 09:06:55', NULL);
INSERT INTO `stock_trans` VALUES (7795, 0, 7, 1, 174, 5046, 8, 25, 4, 11, 11, 17, '2020-12-16 08:59:59', '2020-12-16 09:02:42', '2020-12-16 09:05:06', b'1', '2020-12-16 09:05:07', NULL);
INSERT INTO `stock_trans` VALUES (7796, 0, 7, 1, 163, 5047, 9, 39, 5, 12, 12, 16, '2020-12-16 09:02:33', '2020-12-16 09:04:18', '2020-12-16 09:06:38', b'1', '2020-12-16 09:06:39', NULL);
INSERT INTO `stock_trans` VALUES (7797, 0, 7, 1, 174, 5048, 5, 25, 3, 11, 11, 17, '2020-12-16 09:05:08', '2020-12-16 09:06:38', '2020-12-16 09:08:23', b'1', '2020-12-16 09:08:24', NULL);
INSERT INTO `stock_trans` VALUES (7798, 1, 7, 1, 163, 4987, 42, 50, 10, 13, 13, 19, '2020-12-16 09:08:20', '2020-12-16 09:08:21', '2020-12-16 09:10:21', b'1', '2020-12-16 09:11:33', NULL);
INSERT INTO `stock_trans` VALUES (7799, 0, 7, 1, 174, 5049, 2, 25, 1, 11, 11, 17, '2020-12-16 09:08:24', '2020-12-16 09:10:35', '2020-12-16 09:12:14', b'1', '2020-12-16 09:12:15', NULL);
INSERT INTO `stock_trans` VALUES (7800, 1, 7, 1, 174, 4988, 22, 17, 9, 13, 13, 15, '2020-12-16 09:08:33', '2020-12-16 09:08:34', '2020-12-16 09:13:46', b'1', '2020-12-16 09:14:40', NULL);
INSERT INTO `stock_trans` VALUES (7801, 0, 7, 1, 174, 5050, 1, 21, 1, 11, 11, 17, '2020-12-16 09:15:28', '2020-12-16 09:17:05', '2020-12-16 09:19:15', b'1', '2020-12-16 09:19:16', NULL);
INSERT INTO `stock_trans` VALUES (7802, 1, 7, 1, 174, 4989, 22, 18, 9, 13, 13, 15, '2020-12-16 09:16:08', '2020-12-16 09:16:08', '2020-12-16 09:21:57', b'1', '2020-12-16 09:22:58', NULL);
INSERT INTO `stock_trans` VALUES (7803, 1, 7, 1, 163, 4990, 42, 49, 10, 13, 13, 19, '2020-12-16 09:16:37', '2020-12-16 09:16:38', '2020-12-16 09:25:33', b'1', '2020-12-16 09:27:00', NULL);
INSERT INTO `stock_trans` VALUES (7804, 0, 7, 1, 176, 5051, 16, 38, 8, 12, 12, 18, '2020-12-16 09:19:48', '2020-12-16 09:22:15', '2020-12-16 09:25:04', b'1', '2020-12-16 09:25:05', NULL);
INSERT INTO `stock_trans` VALUES (7805, 1, 7, 1, 174, 4992, 22, 17, 9, 13, 13, 15, '2020-12-16 09:25:07', '2020-12-16 09:25:08', '2020-12-16 09:29:12', b'1', '2020-12-16 09:30:06', NULL);
INSERT INTO `stock_trans` VALUES (7806, 0, 7, 1, 174, 5052, 7, 21, 4, 11, 11, 17, '2020-12-16 09:25:58', '2020-12-16 09:27:53', '2020-12-16 09:30:29', b'1', '2020-12-16 09:30:30', NULL);
INSERT INTO `stock_trans` VALUES (7807, 1, 7, 1, 163, 4993, 41, 50, 10, 13, 13, 19, '2020-12-16 09:28:33', '2020-12-16 09:28:34', '2020-12-16 09:32:31', b'1', '2020-12-16 09:33:40', NULL);
INSERT INTO `stock_trans` VALUES (7808, 0, 7, 1, 176, 5053, 15, 38, 8, 12, 12, 18, '2020-12-16 09:30:16', '2020-12-16 09:31:55', '2020-12-16 09:34:21', b'1', '2020-12-16 09:34:22', NULL);
INSERT INTO `stock_trans` VALUES (7809, 0, 7, 1, 174, 5054, 6, 21, 3, 11, 11, 17, '2020-12-16 09:30:31', '2020-12-16 09:32:55', '2020-12-16 09:35:22', b'1', '2020-12-16 09:35:23', NULL);
INSERT INTO `stock_trans` VALUES (7810, 0, 7, 1, 176, 5055, 12, 38, 6, 12, 12, 18, '2020-12-16 09:34:23', '2020-12-16 09:36:23', '2020-12-16 09:38:30', b'1', '2020-12-16 09:38:31', NULL);
INSERT INTO `stock_trans` VALUES (7811, 1, 7, 1, 174, 4994, 22, 18, 9, 13, 13, 15, '2020-12-16 09:34:40', '2020-12-16 09:34:41', '2020-12-16 09:36:31', b'1', '2020-12-16 09:37:33', NULL);
INSERT INTO `stock_trans` VALUES (7812, 0, 7, 1, 174, 5056, 2, 21, 1, 11, 11, 17, '2020-12-16 09:35:23', '2020-12-16 09:37:24', '2020-12-16 09:39:17', b'1', '2020-12-16 09:39:18', NULL);
INSERT INTO `stock_trans` VALUES (7813, 1, 7, 1, 163, 4995, 41, 49, 10, 13, 13, 19, '2020-12-16 09:36:27', '2020-12-16 09:36:27', '2020-12-16 09:40:00', b'1', '2020-12-16 09:41:15', NULL);
INSERT INTO `stock_trans` VALUES (7814, 0, 7, 1, 176, 5057, 10, 38, 5, 12, 12, 18, '2020-12-16 09:38:32', '2020-12-16 09:40:44', '2020-12-16 09:42:46', b'1', '2020-12-16 09:42:47', NULL);
INSERT INTO `stock_trans` VALUES (7815, 0, 7, 1, 174, 5058, 1, 21, 1, 11, 11, 17, '2020-12-16 09:39:20', '2020-12-16 09:40:44', '2020-12-16 09:42:02', b'1', '2020-12-16 09:42:03', NULL);
INSERT INTO `stock_trans` VALUES (7816, 0, 7, 1, 176, 5059, 16, 38, 8, 12, 12, 18, '2020-12-16 09:42:48', '2020-12-16 09:44:42', '2020-12-16 09:46:28', b'1', '2020-12-16 09:46:29', NULL);
INSERT INTO `stock_trans` VALUES (7817, 1, 7, 1, 174, 4997, 22, 17, 9, 13, 13, 15, '2020-12-16 09:43:18', '2020-12-16 09:43:18', '2020-12-16 09:45:02', b'1', '2020-12-16 09:46:17', NULL);
INSERT INTO `stock_trans` VALUES (7818, 1, 7, 1, 163, 4996, 41, 50, 10, 13, 13, 19, '2020-12-16 09:45:15', '2020-12-16 09:45:15', '2020-12-16 09:48:24', b'1', '2020-12-16 09:49:33', NULL);
INSERT INTO `stock_trans` VALUES (7819, 0, 7, 1, 174, 5060, 8, 22, 4, 11, 11, 17, '2020-12-16 09:48:32', '2020-12-16 09:51:06', '2020-12-16 09:53:59', b'1', '2020-12-16 09:54:00', NULL);
INSERT INTO `stock_trans` VALUES (7820, 1, 7, 1, 174, 4998, 26, 18, 9, 13, 13, 15, '2020-12-16 09:51:19', '2020-12-16 09:51:19', '2020-12-16 09:52:54', b'1', '2020-12-16 09:53:52', NULL);
INSERT INTO `stock_trans` VALUES (7821, 0, 7, 1, 176, 5061, 15, 42, 8, 12, 12, 16, '2020-12-16 09:52:10', '2020-12-16 09:54:04', '2020-12-16 09:56:19', b'1', '2020-12-16 09:56:20', NULL);
INSERT INTO `stock_trans` VALUES (7822, 1, 7, 1, 163, 5001, 41, 49, 10, 13, 13, 19, '2020-12-16 09:52:48', '2020-12-16 09:52:49', '2020-12-16 09:56:09', b'1', '2020-12-16 09:57:24', NULL);
INSERT INTO `stock_trans` VALUES (7823, 0, 7, 1, 174, 5062, 2, 22, 1, 11, 11, 17, '2020-12-16 09:54:00', '2020-12-16 09:55:59', '2020-12-16 09:58:11', b'1', '2020-12-16 09:58:12', NULL);
INSERT INTO `stock_trans` VALUES (7824, 0, 7, 1, 176, 5063, 9, 42, 5, 12, 12, 16, '2020-12-16 09:56:20', '2020-12-16 09:58:15', '2020-12-16 10:00:47', b'1', '2020-12-16 10:00:48', NULL);
INSERT INTO `stock_trans` VALUES (7825, 0, 7, 1, 174, 5064, 5, 22, 3, 11, 11, 17, '2020-12-16 09:58:13', '2020-12-16 09:59:53', '2020-12-16 10:02:00', b'1', '2020-12-16 10:02:01', NULL);
INSERT INTO `stock_trans` VALUES (7826, 0, 7, 1, 176, 5065, 11, 42, 6, 12, 12, 16, '2020-12-16 10:00:49', '2020-12-16 10:02:33', '2020-12-16 10:04:43', b'1', '2020-12-16 10:04:44', NULL);
INSERT INTO `stock_trans` VALUES (7827, 1, 7, 1, 174, 4999, 26, 17, 9, 13, 13, 15, '2020-12-16 10:00:56', '2020-12-16 10:00:57', '2020-12-16 10:02:33', b'1', '2020-12-16 10:03:35', NULL);
INSERT INTO `stock_trans` VALUES (7828, 1, 7, 1, 163, 5003, 41, 50, 10, 13, 13, 19, '2020-12-16 10:01:52', '2020-12-16 10:01:53', '2020-12-16 10:05:46', b'1', '2020-12-16 10:07:13', NULL);
INSERT INTO `stock_trans` VALUES (7829, 0, 7, 1, 176, 5066, 16, 42, 8, 12, 12, 16, '2020-12-16 10:04:45', '2020-12-16 10:06:16', '2020-12-16 10:08:00', b'1', '2020-12-16 10:08:01', NULL);
INSERT INTO `stock_trans` VALUES (7830, 1, 7, 1, 174, 5002, 26, 18, 9, 13, 13, 15, '2020-12-16 10:08:46', '2020-12-16 10:08:47', '2020-12-16 10:10:18', b'1', '2020-12-16 10:11:13', NULL);
INSERT INTO `stock_trans` VALUES (7831, 0, 7, 1, 174, 5067, 1, 22, 1, 11, 11, 17, '2020-12-16 10:10:24', '2020-12-16 10:11:50', '2020-12-16 10:13:27', b'1', '2020-12-16 10:13:28', NULL);
INSERT INTO `stock_trans` VALUES (7832, 1, 7, 1, 163, 5004, 44, 49, 10, 13, 13, 19, '2020-12-16 10:11:07', '2020-12-16 10:11:08', '2020-12-16 10:13:45', b'1', '2020-12-16 10:15:05', NULL);
INSERT INTO `stock_trans` VALUES (7833, 0, 7, 1, 176, 5068, 15, 42, 8, 12, 12, 16, '2020-12-16 10:15:20', '2020-12-16 10:16:46', '2020-12-16 10:18:03', b'1', '2020-12-16 10:18:04', NULL);
INSERT INTO `stock_trans` VALUES (7834, 0, 7, 1, 178, 5069, 7, 28, 4, 11, 11, 17, '2020-12-16 10:16:42', '2020-12-16 10:18:34', '2020-12-16 10:20:55', b'1', '2020-12-16 10:20:56', NULL);
INSERT INTO `stock_trans` VALUES (7835, 0, 7, 1, 179, 5070, 3, 29, 2, 11, 11, 18, '2020-12-16 10:17:27', '2020-12-16 10:23:46', '2020-12-16 10:26:24', b'1', '2020-12-16 10:26:25', NULL);
INSERT INTO `stock_trans` VALUES (7836, 1, 7, 1, 174, 5005, 26, 17, 9, 13, 13, 15, '2020-12-16 10:17:56', '2020-12-16 10:17:56', '2020-12-16 10:19:34', b'1', '2020-12-16 10:20:36', NULL);
INSERT INTO `stock_trans` VALUES (7837, 0, 7, 1, 176, 5071, 12, 41, 6, 12, 12, 16, '2020-12-16 10:18:05', '2020-12-16 10:20:10', '2020-12-16 10:22:47', b'1', '2020-12-16 10:22:48', NULL);
INSERT INTO `stock_trans` VALUES (7838, 1, 7, 1, 163, 5009, 44, 50, 10, 13, 13, 19, '2020-12-16 10:18:36', '2020-12-16 10:18:37', '2020-12-16 10:23:01', b'1', '2020-12-16 10:24:19', NULL);
INSERT INTO `stock_trans` VALUES (7839, 0, 7, 1, 176, 5072, 10, 41, 5, 12, 12, 16, '2020-12-16 10:22:49', '2020-12-16 10:25:16', '2020-12-16 10:27:48', b'1', '2020-12-16 10:27:49', NULL);
INSERT INTO `stock_trans` VALUES (7840, 1, 7, 1, 174, 5006, 26, 18, 9, 13, 13, 15, '2020-12-16 10:25:27', '2020-12-16 10:25:27', '2020-12-16 10:29:40', b'1', '2020-12-16 10:30:49', NULL);
INSERT INTO `stock_trans` VALUES (7841, 1, 7, 1, 163, 5011, 44, 49, 10, 13, 13, 19, '2020-12-16 10:26:53', '2020-12-16 10:26:54', '2020-12-16 10:33:29', b'1', '2020-12-16 10:34:51', NULL);
INSERT INTO `stock_trans` VALUES (7842, 0, 7, 1, 178, 5073, 6, 28, 3, 11, 11, 17, '2020-12-16 10:27:18', '2020-12-16 10:29:16', '2020-12-16 10:31:12', b'1', '2020-12-16 10:31:13', NULL);
INSERT INTO `stock_trans` VALUES (7843, 0, 7, 1, 176, 5074, 16, 41, 8, 12, 12, 16, '2020-12-16 10:27:51', '2020-12-16 10:30:02', '2020-12-16 10:32:32', b'1', '2020-12-16 10:32:33', NULL);
INSERT INTO `stock_trans` VALUES (7844, 1, 7, 1, 163, 5014, 44, 50, 10, 13, 13, 19, '2020-12-16 10:35:14', '2020-12-16 10:35:32', '2020-12-16 10:41:57', b'1', '2020-12-16 10:43:14', NULL);
INSERT INTO `stock_trans` VALUES (7845, 1, 7, 1, 174, 5007, 24, 17, 9, 13, 13, 15, '2020-12-16 10:35:28', '2020-12-16 10:35:28', '2020-12-16 10:37:30', b'1', '2020-12-16 10:38:27', NULL);
INSERT INTO `stock_trans` VALUES (7846, 0, 7, 1, 176, 5075, 15, 41, 8, 12, 12, 16, '2020-12-16 10:37:06', '2020-12-16 10:38:32', '2020-12-16 10:40:17', b'1', '2020-12-16 10:40:18', NULL);
INSERT INTO `stock_trans` VALUES (7847, 0, 7, 1, 178, 5076, 1, 26, 1, 11, 11, 17, '2020-12-16 10:38:49', '2020-12-16 10:40:35', '2020-12-16 10:43:01', b'1', '2020-12-16 10:43:02', NULL);
INSERT INTO `stock_trans` VALUES (7848, 0, 7, 1, 176, 5077, 11, 41, 6, 12, 12, 16, '2020-12-16 10:40:19', '2020-12-16 10:41:59', '2020-12-16 10:43:33', b'1', '2020-12-16 10:43:34', NULL);
INSERT INTO `stock_trans` VALUES (7849, 1, 7, 1, 174, 5008, 24, 18, 9, 13, 13, 15, '2020-12-16 10:43:17', '2020-12-16 10:43:18', '2020-12-16 10:45:31', b'1', '2020-12-16 10:46:26', NULL);
INSERT INTO `stock_trans` VALUES (7850, 0, 7, 1, 176, 5078, 9, 36, 5, 12, 12, 16, '2020-12-16 10:43:36', '2020-12-16 10:45:28', '2020-12-16 10:47:51', b'1', '2020-12-16 10:47:52', NULL);
INSERT INTO `stock_trans` VALUES (7851, 1, 7, 1, 163, 5015, 44, 49, 10, 13, 13, 19, '2020-12-16 10:46:42', '2020-12-16 10:46:42', '2020-12-16 10:49:19', b'1', '2020-12-16 10:50:33', NULL);
INSERT INTO `stock_trans` VALUES (7852, 1, 7, 1, 174, 5010, 24, 17, 9, 13, 13, 15, '2020-12-16 10:51:05', '2020-12-16 10:51:05', '2020-12-16 10:52:43', b'1', '2020-12-16 10:53:38', NULL);
INSERT INTO `stock_trans` VALUES (7853, 0, 7, 1, 176, 5079, 16, 36, 8, 12, 12, 16, '2020-12-16 10:52:27', '2020-12-16 10:54:55', '2020-12-16 10:57:29', b'1', '2020-12-16 10:57:30', NULL);
INSERT INTO `stock_trans` VALUES (7854, 1, 7, 1, 163, 5017, 37, 50, 10, 13, 13, 19, '2020-12-16 10:54:59', '2020-12-16 10:55:00', '2020-12-16 10:56:33', b'1', '2020-12-16 10:57:30', NULL);
INSERT INTO `stock_trans` VALUES (7855, 0, 7, 1, 178, 5080, 2, 26, 1, 11, 11, 17, '2020-12-16 10:56:12', '2020-12-16 10:58:23', '2020-12-16 11:00:49', b'1', '2020-12-16 11:00:50', NULL);
INSERT INTO `stock_trans` VALUES (7856, 1, 7, 1, 174, 5012, 24, 18, 9, 13, 13, 15, '2020-12-16 10:59:24', '2020-12-16 10:59:24', '2020-12-16 11:01:01', b'1', '2020-12-16 11:01:56', NULL);
INSERT INTO `stock_trans` VALUES (7857, 0, 7, 1, 178, 5081, 8, 26, 4, 11, 11, 17, '2020-12-16 11:00:51', '2020-12-16 11:03:07', '2020-12-16 11:05:26', b'1', '2020-12-16 11:05:27', NULL);
INSERT INTO `stock_trans` VALUES (7858, 1, 7, 1, 163, 5026, 37, 49, 10, 13, 13, 19, '2020-12-16 11:04:01', '2020-12-16 11:04:02', '2020-12-16 11:05:41', b'1', '2020-12-16 11:06:45', NULL);
INSERT INTO `stock_trans` VALUES (7859, 0, 7, 1, 176, 5082, 15, 36, 8, 12, 12, 16, '2020-12-16 11:04:28', '2020-12-16 11:06:17', '2020-12-16 11:08:33', b'1', '2020-12-16 11:08:34', NULL);
INSERT INTO `stock_trans` VALUES (7860, 0, 7, 1, 178, 5083, 5, 26, 3, 11, 11, 17, '2020-12-16 11:05:27', '2020-12-16 11:06:53', '2020-12-16 11:08:33', b'1', '2020-12-16 11:08:34', NULL);
INSERT INTO `stock_trans` VALUES (7861, 1, 7, 1, 174, 5013, 24, 17, 9, 13, 13, 15, '2020-12-16 11:07:25', '2020-12-16 11:07:25', '2020-12-16 11:09:03', b'1', '2020-12-16 11:10:25', NULL);
INSERT INTO `stock_trans` VALUES (7862, 0, 7, 1, 176, 5084, 10, 36, 5, 12, 12, 16, '2020-12-16 11:08:34', '2020-12-16 11:10:38', '2020-12-16 11:12:31', b'1', '2020-12-16 11:12:32', NULL);
INSERT INTO `stock_trans` VALUES (7863, 0, 7, 1, 178, 5085, 1, 26, 1, 11, 11, 17, '2020-12-16 11:08:36', '2020-12-16 11:10:14', '2020-12-16 11:11:52', b'1', '2020-12-16 11:11:53', NULL);
INSERT INTO `stock_trans` VALUES (7864, 1, 7, 1, 163, 5027, 37, 50, 10, 13, 13, 19, '2020-12-16 11:11:44', '2020-12-16 11:11:45', '2020-12-16 11:13:16', b'1', '2020-12-16 11:14:12', NULL);
INSERT INTO `stock_trans` VALUES (7865, 0, 7, 1, 176, 5086, 12, 36, 6, 12, 12, 16, '2020-12-16 11:12:34', '2020-12-16 11:14:31', '2020-12-16 11:16:02', b'1', '2020-12-16 11:16:03', NULL);
INSERT INTO `stock_trans` VALUES (7866, 1, 7, 1, 174, 5016, 27, 18, 9, 13, 13, 15, '2020-12-16 11:15:01', '2020-12-16 11:15:02', '2020-12-16 11:16:36', b'1', '2020-12-16 11:17:32', NULL);
INSERT INTO `stock_trans` VALUES (7867, 0, 7, 1, 176, 5087, 16, 44, 8, 12, 12, 16, '2020-12-16 11:16:04', '2020-12-16 11:18:31', '2020-12-16 11:20:50', b'1', '2020-12-16 11:20:51', NULL);
INSERT INTO `stock_trans` VALUES (7868, 1, 7, 1, 163, 5029, 37, 49, 10, 13, 13, 19, '2020-12-16 11:19:38', '2020-12-16 11:19:39', '2020-12-16 11:21:16', b'1', '2020-12-16 11:22:18', NULL);
INSERT INTO `stock_trans` VALUES (7869, 1, 7, 1, 174, 5018, 27, 17, 9, 13, 13, 15, '2020-12-16 11:23:06', '2020-12-16 11:23:07', '2020-12-16 11:24:44', b'1', '2020-12-16 11:25:49', NULL);
INSERT INTO `stock_trans` VALUES (7870, 0, 7, 1, 178, 5088, 2, 24, 1, 11, 11, 17, '2020-12-16 11:24:27', '2020-12-16 11:26:40', '2020-12-16 11:29:09', b'1', '2020-12-16 11:29:10', NULL);
INSERT INTO `stock_trans` VALUES (7871, 0, 7, 1, 176, 5089, 15, 44, 8, 12, 12, 16, '2020-12-16 11:26:37', '2020-12-16 11:28:10', '2020-12-16 11:30:17', b'1', '2020-12-16 11:30:18', NULL);
INSERT INTO `stock_trans` VALUES (7872, 1, 7, 1, 163, 5033, 37, 50, 10, 13, 13, 19, '2020-12-16 11:28:21', '2020-12-16 11:28:22', '2020-12-16 11:29:54', b'1', '2020-12-16 11:31:22', NULL);
INSERT INTO `stock_trans` VALUES (7873, 0, 7, 1, 178, 5090, 6, 24, 3, 11, 11, 17, '2020-12-16 11:29:11', '2020-12-16 11:31:20', '2020-12-16 11:33:45', b'1', '2020-12-16 11:33:46', NULL);
INSERT INTO `stock_trans` VALUES (7874, 0, 7, 1, 176, 5091, 11, 44, 6, 12, 12, 16, '2020-12-16 11:30:19', '2020-12-16 11:32:11', '2020-12-16 11:34:34', b'1', '2020-12-16 11:34:35', NULL);
INSERT INTO `stock_trans` VALUES (7875, 1, 7, 1, 174, 5019, 27, 18, 9, 13, 13, 15, '2020-12-16 11:31:17', '2020-12-16 11:31:17', '2020-12-16 11:33:26', b'1', '2020-12-16 11:34:23', NULL);
INSERT INTO `stock_trans` VALUES (7876, 0, 7, 1, 178, 5092, 7, 24, 4, 11, 11, 17, '2020-12-16 11:33:47', '2020-12-16 11:35:31', '2020-12-16 11:37:46', b'1', '2020-12-16 11:37:47', NULL);
INSERT INTO `stock_trans` VALUES (7877, 0, 7, 1, 176, 5093, 9, 44, 5, 12, 12, 16, '2020-12-16 11:34:35', '2020-12-16 11:36:35', '2020-12-16 11:38:45', b'1', '2020-12-16 11:38:46', NULL);
INSERT INTO `stock_trans` VALUES (7878, 1, 7, 1, 163, 5036, 43, 49, 10, 13, 13, 19, '2020-12-16 11:36:31', '2020-12-16 11:36:32', '2020-12-16 11:38:39', b'1', '2020-12-16 11:39:59', NULL);
INSERT INTO `stock_trans` VALUES (7879, 0, 7, 1, 178, 5094, 1, 24, 1, 11, 11, 17, '2020-12-16 11:37:47', '2020-12-16 11:39:17', '2020-12-16 11:41:05', b'1', '2020-12-16 11:41:06', NULL);
INSERT INTO `stock_trans` VALUES (7880, 0, 7, 1, 176, 5095, 16, 44, 8, 12, 12, 16, '2020-12-16 11:38:47', '2020-12-16 11:40:48', '2020-12-16 11:42:11', b'1', '2020-12-16 11:42:12', NULL);
INSERT INTO `stock_trans` VALUES (7881, 1, 7, 1, 174, 5020, 27, 17, 9, 13, 13, 15, '2020-12-16 11:38:57', '2020-12-16 11:38:57', '2020-12-16 11:42:07', b'1', '2020-12-16 11:43:13', NULL);
INSERT INTO `stock_trans` VALUES (7882, 1, 7, 1, 163, 5038, 43, 50, 10, 13, 13, 19, '2020-12-16 11:47:00', '2020-12-16 11:47:00', '2020-12-16 11:49:02', b'1', '2020-12-16 11:50:15', NULL);
INSERT INTO `stock_trans` VALUES (7883, 1, 7, 1, 174, 5021, 27, 18, 9, 13, 13, 15, '2020-12-16 11:47:08', '2020-12-16 11:47:08', '2020-12-16 11:52:16', b'1', '2020-12-16 11:53:40', NULL);
INSERT INTO `stock_trans` VALUES (7884, 0, 7, 1, 176, 5096, 15, 37, 8, 12, 12, 16, '2020-12-16 11:48:24', '2020-12-16 11:49:56', '2020-12-16 11:52:26', b'1', '2020-12-16 11:52:27', NULL);
INSERT INTO `stock_trans` VALUES (7885, 0, 7, 1, 178, 5097, 2, 24, 1, 11, 11, 17, '2020-12-16 11:49:50', '2020-12-16 11:51:54', '2020-12-16 11:53:23', b'1', '2020-12-16 11:53:24', NULL);
INSERT INTO `stock_trans` VALUES (7886, 0, 7, 1, 176, 5098, 10, 37, 5, 12, 12, 16, '2020-12-16 11:52:27', '2020-12-16 11:54:32', '2020-12-16 11:56:52', b'1', '2020-12-16 11:56:53', NULL);
INSERT INTO `stock_trans` VALUES (7887, 0, 7, 1, 178, 5099, 5, 30, 3, 11, 11, 17, '2020-12-16 11:53:24', '2020-12-16 11:54:56', '2020-12-16 11:57:18', b'1', '2020-12-16 11:57:19', NULL);
INSERT INTO `stock_trans` VALUES (7888, 1, 7, 1, 174, 5022, 19, 17, 9, 13, 13, 15, '2020-12-16 11:55:06', '2020-12-16 11:55:06', '2020-12-16 11:57:06', b'1', '2020-12-16 11:58:10', NULL);
INSERT INTO `stock_trans` VALUES (7889, 1, 7, 1, 163, 5040, 43, 49, 10, 13, 13, 19, '2020-12-16 11:56:36', '2020-12-16 11:56:37', '2020-12-16 12:00:56', b'1', '2020-12-16 12:02:16', NULL);
INSERT INTO `stock_trans` VALUES (7890, 0, 7, 1, 176, 5100, 12, 37, 6, 12, 12, 16, '2020-12-16 11:56:54', '2020-12-16 11:58:47', '2020-12-16 12:00:44', b'1', '2020-12-16 12:00:45', NULL);
INSERT INTO `stock_trans` VALUES (7891, 0, 7, 1, 178, 5101, 8, 30, 4, 11, 11, 17, '2020-12-16 11:57:21', '2020-12-16 11:59:21', '2020-12-16 12:01:34', b'1', '2020-12-16 12:01:35', NULL);
INSERT INTO `stock_trans` VALUES (7892, 0, 7, 1, 176, 5102, 16, 37, 8, 12, 12, 16, '2020-12-16 12:00:46', '2020-12-16 12:03:09', '2020-12-16 12:06:46', b'1', '2020-12-16 12:06:47', NULL);
INSERT INTO `stock_trans` VALUES (7893, 1, 7, 1, 163, 5041, 43, 50, 10, 13, 13, 19, '2020-12-16 12:04:18', '2020-12-16 12:04:19', '2020-12-16 12:06:20', b'1', '2020-12-16 12:07:34', NULL);
INSERT INTO `stock_trans` VALUES (7894, 1, 7, 1, 174, 5023, 19, 18, 9, 13, 13, 15, '2020-12-16 12:05:56', '2020-12-16 12:05:56', '2020-12-16 12:10:09', b'1', '2020-12-16 12:11:17', NULL);
INSERT INTO `stock_trans` VALUES (7895, 0, 7, 1, 176, 5103, 15, 37, 8, 12, 12, 16, '2020-12-16 12:10:11', '2020-12-16 12:11:52', '2020-12-16 12:13:27', b'1', '2020-12-16 12:13:28', NULL);
INSERT INTO `stock_trans` VALUES (7896, 1, 7, 1, 163, 5043, 43, 49, 10, 13, 13, 19, '2020-12-16 12:13:02', '2020-12-16 12:13:02', '2020-12-16 12:15:17', b'1', '2020-12-16 12:16:38', NULL);
INSERT INTO `stock_trans` VALUES (7897, 0, 7, 1, 178, 5104, 1, 30, 1, 11, 11, 17, '2020-12-16 12:14:07', '2020-12-16 12:16:03', '2020-12-16 12:18:27', b'1', '2020-12-16 12:18:28', NULL);
INSERT INTO `stock_trans` VALUES (7898, 1, 7, 1, 174, 5024, 19, 17, 9, 13, 13, 15, '2020-12-16 12:14:49', '2020-12-16 12:14:50', '2020-12-16 12:18:57', b'1', '2020-12-16 12:20:00', NULL);
INSERT INTO `stock_trans` VALUES (7899, 0, 7, 1, 176, 5105, 9, 35, 5, 12, 12, 16, '2020-12-16 12:16:14', '2020-12-16 12:17:51', '2020-12-16 12:20:09', b'1', '2020-12-16 12:20:10', NULL);
INSERT INTO `stock_trans` VALUES (7900, 0, 7, 1, 176, 5106, 11, 35, 6, 12, 12, 16, '2020-12-16 12:20:12', '2020-12-16 12:21:35', '2020-12-16 12:23:40', b'1', '2020-12-16 12:23:41', NULL);
INSERT INTO `stock_trans` VALUES (7901, 1, 7, 1, 163, 5045, 39, 50, 10, 13, 13, 19, '2020-12-16 12:21:45', '2020-12-16 12:21:45', '2020-12-16 12:23:34', b'1', '2020-12-16 12:24:36', NULL);
INSERT INTO `stock_trans` VALUES (7902, 1, 7, 1, 174, 5025, 19, 18, 9, 13, 13, 15, '2020-12-16 12:22:29', '2020-12-16 12:22:29', '2020-12-16 12:27:03', b'1', '2020-12-16 12:28:12', NULL);
INSERT INTO `stock_trans` VALUES (7903, 0, 7, 1, 176, 5107, 16, 35, 8, 12, 12, 16, '2020-12-16 12:23:41', '2020-12-16 12:26:08', '2020-12-16 12:28:37', b'1', '2020-12-16 12:28:38', NULL);
INSERT INTO `stock_trans` VALUES (7904, 1, 7, 1, 163, 5047, 39, 49, 10, 13, 13, 19, '2020-12-16 12:24:38', '2020-12-16 12:25:05', '2020-12-16 12:30:37', b'1', '2020-12-16 12:31:58', NULL);
INSERT INTO `stock_trans` VALUES (7905, 0, 7, 1, 178, 5108, 2, 30, 1, 11, 11, 17, '2020-12-16 12:25:48', '2020-12-16 12:28:15', '2020-12-16 12:30:29', b'1', '2020-12-16 12:30:30', NULL);
INSERT INTO `stock_trans` VALUES (7906, 0, 7, 1, 178, 5109, 7, 30, 4, 11, 11, 17, '2020-12-16 12:30:32', '2020-12-16 12:31:51', '2020-12-16 12:33:04', b'1', '2020-12-16 12:33:05', NULL);
INSERT INTO `stock_trans` VALUES (7907, 1, 7, 1, 174, 5028, 19, 17, 9, 13, 13, 15, '2020-12-16 12:31:08', '2020-12-16 12:31:09', '2020-12-16 12:34:18', b'1', '2020-12-16 12:35:32', NULL);
INSERT INTO `stock_trans` VALUES (7908, 1, 7, 1, 164, 4938, 40, 50, 10, 13, 13, 18, '2020-12-16 12:32:00', '2020-12-16 12:37:56', '2020-12-16 12:39:10', b'1', '2020-12-16 12:40:17', NULL);
INSERT INTO `stock_trans` VALUES (7909, 0, 7, 1, 178, 5110, 6, 31, 3, 11, 11, 17, '2020-12-16 12:33:05', '2020-12-16 12:35:13', '2020-12-16 12:37:46', b'1', '2020-12-16 12:37:47', NULL);
INSERT INTO `stock_trans` VALUES (7910, 0, 7, 1, 176, 5111, 15, 35, 8, 12, 12, 16, '2020-12-16 12:35:58', '2020-12-16 12:37:46', '2020-12-16 12:39:48', b'1', '2020-12-16 12:39:49', NULL);
INSERT INTO `stock_trans` VALUES (7911, 0, 7, 1, 176, 5112, 10, 35, 5, 12, 12, 16, '2020-12-16 12:39:58', '2020-12-16 12:41:58', '2020-12-16 12:43:24', b'1', '2020-12-16 12:43:25', NULL);
INSERT INTO `stock_trans` VALUES (7912, 1, 7, 1, 164, 4991, 40, 49, 10, 13, 13, 18, '2020-12-16 12:40:19', '2020-12-16 12:40:51', '2020-12-16 12:47:01', b'1', '2020-12-16 12:48:15', NULL);
INSERT INTO `stock_trans` VALUES (7913, 0, 7, 1, 178, 5113, 1, 31, 1, 11, 11, 17, '2020-12-16 12:40:21', '2020-12-16 12:42:23', '2020-12-16 12:45:02', b'1', '2020-12-16 12:45:03', NULL);
INSERT INTO `stock_trans` VALUES (7914, 1, 7, 1, 174, 5030, 20, 18, 9, 13, 13, 15, '2020-12-16 12:40:59', '2020-12-16 12:41:00', '2020-12-16 12:43:22', b'1', '2020-12-16 12:44:28', NULL);
INSERT INTO `stock_trans` VALUES (7915, 0, 7, 1, 176, 5114, 12, 43, 6, 12, 12, 19, '2020-12-16 12:43:26', '2020-12-16 12:45:59', '2020-12-16 12:48:34', b'1', '2020-12-16 12:48:35', NULL);
INSERT INTO `stock_trans` VALUES (7916, 1, 7, 1, 164, 5044, 40, 50, 10, 13, 13, 18, '2020-12-16 12:48:17', '2020-12-16 12:48:54', '2020-12-16 12:54:23', b'1', '2020-12-16 12:55:45', NULL);
INSERT INTO `stock_trans` VALUES (7917, 0, 7, 1, 176, 5115, 16, 43, 8, 12, 12, 19, '2020-12-16 12:48:37', '2020-12-16 12:50:27', '2020-12-16 12:52:31', b'1', '2020-12-16 12:52:32', NULL);
INSERT INTO `stock_trans` VALUES (7918, 1, 7, 1, 174, 5031, 20, 17, 9, 13, 13, 15, '2020-12-16 12:48:37', '2020-12-16 12:48:38', '2020-12-16 12:50:55', b'1', '2020-12-16 12:51:54', NULL);
INSERT INTO `stock_trans` VALUES (7919, 0, 7, 1, 178, 5116, 2, 31, 1, 11, 11, 17, '2020-12-16 12:56:08', '2020-12-16 12:58:39', '2020-12-16 13:01:10', b'1', '2020-12-16 13:01:11', NULL);
INSERT INTO `stock_trans` VALUES (7920, 1, 7, 1, 174, 5032, 20, 18, 9, 13, 13, 15, '2020-12-16 12:56:23', '2020-12-16 12:56:24', '2020-12-16 12:58:41', b'1', '2020-12-16 12:59:47', NULL);
INSERT INTO `stock_trans` VALUES (7921, 0, 7, 1, 176, 5117, 15, 43, 8, 12, 12, 19, '2020-12-16 12:57:26', '2020-12-16 12:58:43', '2020-12-16 13:00:28', b'1', '2020-12-16 13:00:29', NULL);
INSERT INTO `stock_trans` VALUES (7922, 0, 7, 1, 178, 5118, 5, 31, 3, 11, 11, 17, '2020-12-16 13:01:13', '2020-12-16 13:02:53', '2020-12-16 13:04:43', b'1', '2020-12-16 13:04:44', NULL);
INSERT INTO `stock_trans` VALUES (7923, 0, 7, 1, 176, 5119, 9, 43, 5, 12, 12, 19, '2020-12-16 13:01:56', '2020-12-16 13:03:49', '2020-12-16 13:05:55', b'1', '2020-12-16 13:05:56', NULL);
INSERT INTO `stock_trans` VALUES (7924, 0, 7, 1, 178, 5120, 8, 31, 4, 11, 11, 17, '2020-12-16 13:04:46', '2020-12-16 13:06:43', '2020-12-16 13:08:08', b'1', '2020-12-16 13:08:09', NULL);
INSERT INTO `stock_trans` VALUES (7925, 1, 7, 1, 174, 5035, 20, 17, 9, 13, 13, 15, '2020-12-16 13:04:55', '2020-12-16 13:04:56', '2020-12-16 13:06:47', b'1', '2020-12-16 13:07:47', NULL);
INSERT INTO `stock_trans` VALUES (7926, 0, 7, 1, 176, 5121, 11, 43, 6, 12, 12, 19, '2020-12-16 13:05:57', '2020-12-16 13:07:40', '2020-12-16 13:09:12', b'1', '2020-12-16 13:09:13', NULL);
INSERT INTO `stock_trans` VALUES (7927, 0, 7, 1, 178, 5122, 1, 19, 1, 11, 11, 17, '2020-12-16 13:08:43', '2020-12-16 13:10:44', '2020-12-16 13:13:00', b'1', '2020-12-16 13:13:01', NULL);
INSERT INTO `stock_trans` VALUES (7928, 0, 7, 1, 176, 5123, 16, 40, 8, 12, 12, 18, '2020-12-16 13:10:36', '2020-12-16 13:13:52', '2020-12-16 13:16:32', b'1', '2020-12-16 13:16:33', NULL);
INSERT INTO `stock_trans` VALUES (7929, 1, 7, 1, 174, 5037, 20, 18, 9, 13, 13, 15, '2020-12-16 13:13:10', '2020-12-16 13:13:10', '2020-12-16 13:16:44', b'1', '2020-12-16 13:17:49', NULL);
INSERT INTO `stock_trans` VALUES (7930, 1, 7, 1, 176, 5051, 38, 49, 10, 13, 13, 17, '2020-12-16 13:14:41', '2020-12-16 13:20:30', '2020-12-16 13:21:46', b'1', '2020-12-16 13:22:55', NULL);
INSERT INTO `stock_trans` VALUES (7931, 0, 7, 1, 177, 5124, 13, 39, 7, 12, 12, 19, '2020-12-16 13:14:48', '2020-12-16 13:16:54', '2020-12-16 13:19:03', b'1', '2020-12-16 13:19:04', NULL);
INSERT INTO `stock_trans` VALUES (7932, 1, 7, 1, 174, 5039, 25, 17, 9, 13, 13, 15, '2020-12-16 13:21:45', '2020-12-16 13:21:46', '2020-12-16 13:24:55', b'1', '2020-12-16 13:25:54', NULL);
INSERT INTO `stock_trans` VALUES (7933, 0, 7, 1, 178, 5125, 2, 19, 1, 11, 11, 16, '2020-12-16 13:21:54', '2020-12-16 13:25:43', '2020-12-16 13:27:57', b'1', '2020-12-16 13:27:58', NULL);
INSERT INTO `stock_trans` VALUES (7934, 0, 7, 1, 176, 5126, 15, 40, 8, 12, 12, 18, '2020-12-16 13:21:55', '2020-12-16 13:23:47', '2020-12-16 13:26:03', b'1', '2020-12-16 13:26:04', NULL);
INSERT INTO `stock_trans` VALUES (7935, 0, 7, 1, 179, 5127, 4, 20, 2, 11, 11, 19, '2020-12-16 13:22:20', '2020-12-16 13:33:39', '2020-12-16 13:36:12', b'1', '2020-12-16 13:36:13', NULL);
INSERT INTO `stock_trans` VALUES (7936, 1, 7, 1, 176, 5053, 38, 50, 10, 13, 13, 17, '2020-12-16 13:22:56', '2020-12-16 13:23:29', '2020-12-16 13:27:53', b'1', '2020-12-16 13:28:57', NULL);
INSERT INTO `stock_trans` VALUES (7937, 0, 7, 1, 176, 5128, 10, 40, 5, 12, 12, 18, '2020-12-16 13:26:05', '2020-12-16 13:28:25', '2020-12-16 13:30:51', b'1', '2020-12-16 13:30:52', NULL);
INSERT INTO `stock_trans` VALUES (7938, 0, 7, 1, 178, 5129, 7, 19, 4, 11, 11, 16, '2020-12-16 13:27:59', '2020-12-16 13:37:10', '2020-12-16 13:39:37', b'1', '2020-12-16 13:39:39', NULL);
INSERT INTO `stock_trans` VALUES (7939, 1, 7, 1, 174, 5042, 25, 18, 9, 13, 13, 15, '2020-12-16 13:29:17', '2020-12-16 13:29:18', '2020-12-16 13:31:07', b'1', '2020-12-16 13:31:57', NULL);
INSERT INTO `stock_trans` VALUES (7940, 0, 7, 1, 176, 5130, 12, 40, 6, 12, 12, 18, '2020-12-16 13:30:52', '2020-12-16 13:33:56', '2020-12-16 13:35:56', b'1', '2020-12-16 13:35:57', NULL);
INSERT INTO `stock_trans` VALUES (7941, 1, 7, 1, 176, 5055, 38, 49, 10, 13, 13, 17, '2020-12-16 13:32:13', '2020-12-16 13:32:14', '2020-12-16 13:34:26', b'1', '2020-12-16 13:35:38', NULL);
INSERT INTO `stock_trans` VALUES (7942, 0, 7, 1, 176, 5131, 16, 40, 8, 12, 12, 18, '2020-12-16 13:35:59', '2020-12-16 13:38:12', '2020-12-16 13:39:49', b'1', '2020-12-16 13:39:50', NULL);
INSERT INTO `stock_trans` VALUES (7943, 1, 7, 1, 174, 5046, 25, 17, 9, 13, 13, 15, '2020-12-16 13:39:06', '2020-12-16 13:39:07', '2020-12-16 13:40:47', b'1', '2020-12-16 13:41:48', NULL);
INSERT INTO `stock_trans` VALUES (7944, 1, 7, 1, 176, 5057, 38, 50, 10, 13, 13, 17, '2020-12-16 13:39:18', '2020-12-16 13:39:18', '2020-12-16 13:43:49', b'1', '2020-12-16 13:44:52', NULL);
INSERT INTO `stock_trans` VALUES (7945, 0, 7, 1, 178, 5132, 1, 19, 1, 11, 11, 16, '2020-12-16 13:39:39', '2020-12-16 13:41:07', '2020-12-16 13:42:47', b'1', '2020-12-16 13:42:48', NULL);
INSERT INTO `stock_trans` VALUES (7946, 0, 7, 1, 178, 5133, 6, 19, 3, 11, 11, 16, '2020-12-16 13:42:50', '2020-12-16 13:45:17', '2020-12-16 13:47:11', b'1', '2020-12-16 13:47:12', NULL);
INSERT INTO `stock_trans` VALUES (7947, 0, 7, 1, 176, 5134, 15, 34, 8, 12, 12, 18, '2020-12-16 13:45:35', '2020-12-16 13:47:11', '2020-12-16 13:49:59', b'1', '2020-12-16 13:50:00', NULL);
INSERT INTO `stock_trans` VALUES (7948, 1, 7, 1, 174, 5048, 25, 18, 9, 13, 13, 15, '2020-12-16 13:47:01', '2020-12-16 13:47:01', '2020-12-16 13:56:41', b'1', '2020-12-16 13:57:33', NULL);
INSERT INTO `stock_trans` VALUES (7949, 0, 7, 1, 178, 5135, 8, 32, 4, 11, 11, 16, '2020-12-16 13:47:13', '2020-12-16 13:49:57', '2020-12-16 13:52:23', b'1', '2020-12-16 13:52:24', NULL);
INSERT INTO `stock_trans` VALUES (7950, 1, 7, 1, 176, 5059, 38, 49, 10, 13, 13, 17, '2020-12-16 13:47:15', '2020-12-16 13:47:15', '2020-12-16 13:59:41', b'1', '2020-12-16 14:01:16', NULL);
INSERT INTO `stock_trans` VALUES (7951, 0, 7, 1, 176, 5136, 9, 34, 5, 12, 12, 18, '2020-12-16 13:50:01', '2020-12-16 13:52:01', '2020-12-16 13:54:11', b'1', '2020-12-16 13:54:12', NULL);
INSERT INTO `stock_trans` VALUES (7952, 0, 7, 1, 178, 5137, 5, 32, 3, 11, 11, 16, '2020-12-16 13:52:25', '2020-12-16 13:54:25', '2020-12-16 13:56:43', b'1', '2020-12-16 13:56:44', NULL);
INSERT INTO `stock_trans` VALUES (7953, 0, 7, 1, 176, 5138, 11, 34, 6, 12, 12, 18, '2020-12-16 13:54:14', '2020-12-16 13:55:46', '2020-12-16 13:57:36', b'1', '2020-12-16 13:57:37', NULL);
INSERT INTO `stock_trans` VALUES (7954, 0, 7, 1, 178, 5139, 2, 32, 1, 11, 11, 16, '2020-12-16 13:56:45', '2020-12-16 13:59:19', '2020-12-16 14:01:49', b'1', '2020-12-16 14:01:50', NULL);
INSERT INTO `stock_trans` VALUES (7955, 1, 7, 1, 174, 5049, 25, 17, 9, 13, 13, 15, '2020-12-16 13:57:34', '2020-12-16 13:58:22', '2020-12-16 14:03:22', b'1', '2020-12-16 14:04:32', NULL);
INSERT INTO `stock_trans` VALUES (7956, 1, 7, 1, 176, 5061, 42, 50, 10, 13, 13, 17, '2020-12-16 14:01:18', '2020-12-16 14:01:29', '2020-12-16 14:07:04', b'1', '2020-12-16 14:08:19', NULL);
INSERT INTO `stock_trans` VALUES (7957, 0, 7, 1, 178, 5140, 1, 32, 1, 11, 11, 16, '2020-12-16 14:01:51', '2020-12-16 14:03:58', '2020-12-16 14:06:09', b'1', '2020-12-16 14:06:10', NULL);
INSERT INTO `stock_trans` VALUES (7958, 1, 7, 1, 174, 5050, 21, 18, 9, 13, 13, 15, '2020-12-16 14:04:34', '2020-12-16 14:04:43', '2020-12-16 14:10:42', b'1', '2020-12-16 14:11:43', NULL);
INSERT INTO `stock_trans` VALUES (7959, 1, 7, 1, 176, 5063, 42, 49, 10, 13, 13, 17, '2020-12-16 14:08:21', '2020-12-16 14:08:50', '2020-12-16 14:14:25', b'1', '2020-12-16 14:15:46', NULL);
INSERT INTO `stock_trans` VALUES (7960, 0, 7, 1, 176, 5141, 16, 34, 8, 12, 12, 18, '2020-12-16 14:10:48', '2020-12-16 14:13:20', '2020-12-16 14:15:38', b'1', '2020-12-16 14:15:39', NULL);
INSERT INTO `stock_trans` VALUES (7961, 1, 7, 1, 174, 5052, 21, 17, 9, 13, 13, 15, '2020-12-16 14:13:37', '2020-12-16 14:13:37', '2020-12-16 14:18:01', b'1', '2020-12-16 14:18:57', NULL);
INSERT INTO `stock_trans` VALUES (7962, 0, 7, 1, 178, 5142, 7, 32, 4, 11, 11, 16, '2020-12-16 14:15:34', '2020-12-16 14:17:03', '2020-12-16 14:18:23', b'1', '2020-12-16 14:18:24', NULL);
INSERT INTO `stock_trans` VALUES (7963, 0, 7, 1, 176, 5143, 12, 34, 6, 12, 12, 18, '2020-12-16 14:15:40', '2020-12-16 14:17:49', '2020-12-16 14:19:29', b'1', '2020-12-16 14:19:30', NULL);
INSERT INTO `stock_trans` VALUES (7964, 1, 7, 1, 176, 5065, 42, 50, 10, 13, 13, 17, '2020-12-16 14:15:48', '2020-12-16 14:16:25', '2020-12-16 14:21:32', b'1', '2020-12-16 14:22:48', NULL);
INSERT INTO `stock_trans` VALUES (7965, 0, 7, 1, 180, 5144, 10, 38, 5, 12, 12, 16, '2020-12-16 14:16:07', '2020-12-16 14:21:30', '2020-12-16 14:24:12', b'1', '2020-12-16 14:24:13', NULL);
INSERT INTO `stock_trans` VALUES (7966, 0, 7, 1, 178, 5145, 2, 25, 1, 11, 11, 19, '2020-12-16 14:18:25', '2020-12-16 14:21:10', '2020-12-16 14:23:39', b'1', '2020-12-16 14:23:40', NULL);
INSERT INTO `stock_trans` VALUES (7967, 1, 7, 1, 174, 5054, 21, 18, 9, 13, 13, 15, '2020-12-16 14:21:19', '2020-12-16 14:21:20', '2020-12-16 14:25:13', b'1', '2020-12-16 14:26:16', NULL);
INSERT INTO `stock_trans` VALUES (7968, 0, 7, 1, 178, 5146, 6, 25, 3, 11, 11, 19, '2020-12-16 14:23:40', '2020-12-16 14:25:41', '2020-12-16 14:27:55', b'1', '2020-12-16 14:27:56', NULL);
INSERT INTO `stock_trans` VALUES (7969, 1, 7, 1, 176, 5066, 42, 49, 10, 13, 13, 17, '2020-12-16 14:24:03', '2020-12-16 14:24:03', '2020-12-16 14:28:57', b'1', '2020-12-16 14:30:18', NULL);
INSERT INTO `stock_trans` VALUES (7970, 0, 7, 1, 178, 5147, 1, 25, 1, 11, 11, 19, '2020-12-16 14:28:46', '2020-12-16 14:30:17', '2020-12-16 14:32:19', b'1', '2020-12-16 14:32:20', NULL);
INSERT INTO `stock_trans` VALUES (7971, 1, 7, 1, 174, 5056, 21, 17, 9, 13, 13, 15, '2020-12-16 14:29:57', '2020-12-16 14:29:58', '2020-12-16 14:32:35', b'1', '2020-12-16 14:33:34', NULL);
INSERT INTO `stock_trans` VALUES (7972, 1, 7, 1, 176, 5068, 42, 50, 10, 13, 13, 17, '2020-12-16 14:32:02', '2020-12-16 14:32:03', '2020-12-16 14:36:05', b'1', '2020-12-16 14:37:30', NULL);
INSERT INTO `stock_trans` VALUES (7973, 0, 7, 1, 180, 5148, 15, 42, 8, 12, 12, 16, '2020-12-16 14:37:38', '2020-12-16 14:39:21', '2020-12-16 14:41:37', b'1', '2020-12-16 14:41:38', NULL);
INSERT INTO `stock_trans` VALUES (7974, 1, 7, 1, 174, 5058, 21, 18, 9, 13, 13, 15, '2020-12-16 14:37:54', '2020-12-16 14:37:54', '2020-12-16 14:40:17', b'1', '2020-12-16 14:41:34', NULL);
INSERT INTO `stock_trans` VALUES (7975, 0, 7, 1, 178, 5149, 8, 25, 4, 11, 11, 19, '2020-12-16 14:38:34', '2020-12-16 14:40:45', '2020-12-16 14:42:48', b'1', '2020-12-16 14:42:49', NULL);
INSERT INTO `stock_trans` VALUES (7976, 1, 7, 1, 176, 5071, 41, 49, 10, 13, 13, 17, '2020-12-16 14:39:09', '2020-12-16 14:39:09', '2020-12-16 14:44:06', b'1', '2020-12-16 14:45:23', NULL);
INSERT INTO `stock_trans` VALUES (7977, 0, 7, 1, 180, 5150, 9, 42, 5, 12, 12, 16, '2020-12-16 14:41:40', '2020-12-16 14:43:24', '2020-12-16 14:45:55', b'1', '2020-12-16 14:45:56', NULL);
INSERT INTO `stock_trans` VALUES (7978, 0, 7, 1, 178, 5151, 2, 25, 1, 11, 11, 19, '2020-12-16 14:42:49', '2020-12-16 14:44:50', '2020-12-16 14:46:17', b'1', '2020-12-16 14:46:18', NULL);
INSERT INTO `stock_trans` VALUES (7979, 0, 7, 1, 178, 5152, 5, 21, 3, 11, 11, 19, '2020-12-16 14:46:19', '2020-12-16 14:47:44', '2020-12-16 14:50:10', b'1', '2020-12-16 14:50:11', NULL);
INSERT INTO `stock_trans` VALUES (7980, 1, 7, 1, 176, 5072, 41, 50, 10, 13, 13, 17, '2020-12-16 14:48:42', '2020-12-16 14:48:42', '2020-12-16 14:50:38', b'1', '2020-12-16 14:51:49', NULL);
INSERT INTO `stock_trans` VALUES (7981, 1, 7, 1, 174, 5060, 22, 17, 9, 13, 13, 15, '2020-12-16 14:49:19', '2020-12-16 14:49:20', '2020-12-16 14:53:58', b'1', '2020-12-16 14:54:53', NULL);
INSERT INTO `stock_trans` VALUES (7982, 0, 7, 1, 178, 5153, 1, 21, 1, 11, 11, 19, '2020-12-16 14:51:42', '2020-12-16 14:53:00', '2020-12-16 14:54:54', b'1', '2020-12-16 14:54:55', NULL);
INSERT INTO `stock_trans` VALUES (7983, 0, 7, 1, 180, 5154, 16, 42, 8, 12, 12, 16, '2020-12-16 14:53:35', '2020-12-16 14:55:14', '2020-12-16 14:57:25', b'1', '2020-12-16 14:57:26', NULL);
INSERT INTO `stock_trans` VALUES (7984, 0, 7, 1, 179, 5155, 14, 38, 7, 12, 12, 18, '2020-12-16 14:54:21', '2020-12-16 14:58:54', '2020-12-16 15:01:30', b'1', '2020-12-16 15:01:32', NULL);
INSERT INTO `stock_trans` VALUES (7985, 1, 7, 1, 176, 5074, 41, 49, 10, 13, 13, 17, '2020-12-16 14:56:19', '2020-12-16 14:56:19', '2020-12-16 14:58:26', b'1', '2020-12-16 14:59:47', NULL);
INSERT INTO `stock_trans` VALUES (7986, 1, 7, 1, 174, 5062, 22, 18, 9, 13, 13, 15, '2020-12-16 14:57:18', '2020-12-16 14:57:19', '2020-12-16 15:02:04', b'1', '2020-12-16 15:03:05', NULL);
INSERT INTO `stock_trans` VALUES (7987, 0, 7, 1, 180, 5156, 10, 42, 5, 12, NULL, 16, '2020-12-16 14:57:27', NULL, NULL, b'1', '2020-12-16 15:05:59', b'1');
INSERT INTO `stock_trans` VALUES (7988, 0, 7, 1, 178, 5157, 7, 21, 4, 11, 11, 19, '2020-12-16 15:01:26', '2020-12-16 15:03:18', '2020-12-16 15:05:34', b'1', '2020-12-16 15:05:36', NULL);
INSERT INTO `stock_trans` VALUES (7989, 1, 7, 1, 176, 5075, 41, 50, 10, 13, 13, 17, '2020-12-16 15:04:05', '2020-12-16 15:04:06', '2020-12-16 15:06:04', b'1', '2020-12-16 15:07:15', NULL);
INSERT INTO `stock_trans` VALUES (7990, 1, 7, 1, 174, 5064, 22, 17, 9, 13, 13, 15, '2020-12-16 15:05:34', '2020-12-16 15:05:35', '2020-12-16 15:09:26', b'1', '2020-12-16 15:10:20', NULL);
INSERT INTO `stock_trans` VALUES (7991, 0, 7, 1, 178, 5158, 6, 21, 3, 11, 11, 19, '2020-12-16 15:05:37', '2020-12-16 15:07:48', '2020-12-16 15:09:50', b'1', '2020-12-16 15:09:51', NULL);
INSERT INTO `stock_trans` VALUES (7992, 0, 7, 1, 178, 5159, 2, 21, 1, 11, 11, 19, '2020-12-16 15:09:52', '2020-12-16 15:11:41', '2020-12-16 15:13:00', b'1', '2020-12-16 15:13:01', NULL);
INSERT INTO `stock_trans` VALUES (7993, 1, 7, 1, 176, 5077, 41, 49, 10, 13, 13, 17, '2020-12-16 15:11:07', '2020-12-16 15:11:07', '2020-12-16 15:13:10', b'1', '2020-12-16 15:14:30', NULL);
INSERT INTO `stock_trans` VALUES (7994, 1, 7, 1, 174, 5067, 22, 18, 9, 13, 13, 15, '2020-12-16 15:13:27', '2020-12-16 15:13:27', '2020-12-16 15:16:31', b'1', '2020-12-16 15:17:44', NULL);
INSERT INTO `stock_trans` VALUES (7995, 0, 7, 1, 178, 5160, 1, 33, 1, 11, 11, 19, '2020-12-16 15:14:20', '2020-12-16 15:15:39', '2020-12-16 15:18:31', b'1', '2020-12-16 15:18:32', NULL);
INSERT INTO `stock_trans` VALUES (7996, 1, 7, 1, 176, 5078, 36, 50, 10, 13, 13, 17, '2020-12-16 15:21:27', '2020-12-16 15:21:28', '2020-12-16 15:22:59', b'1', '2020-12-16 15:23:56', NULL);
INSERT INTO `stock_trans` VALUES (7997, 1, 7, 1, 176, 5079, 36, 49, 10, 13, 13, 17, '2020-12-16 15:29:44', '2020-12-16 15:29:45', '2020-12-16 15:30:57', b'1', '2020-12-16 15:32:01', NULL);
INSERT INTO `stock_trans` VALUES (7998, 0, 7, 1, 178, 5161, 8, 33, 4, 11, 11, 19, '2020-12-16 15:29:52', '2020-12-16 15:31:48', '2020-12-16 15:33:57', b'1', '2020-12-16 15:33:58', NULL);
INSERT INTO `stock_trans` VALUES (7999, 0, 7, 1, 179, 5162, 3, 20, 2, 11, 11, 15, '2020-12-16 15:32:41', '2020-12-16 15:34:57', '2020-12-16 15:37:05', b'1', '2020-12-16 15:37:06', NULL);
INSERT INTO `stock_trans` VALUES (8000, 0, 7, 1, 178, 5163, 5, 33, 3, 11, 11, 19, '2020-12-16 15:33:58', '2020-12-16 15:38:21', '2020-12-16 15:40:24', b'1', '2020-12-16 15:40:25', NULL);
INSERT INTO `stock_trans` VALUES (8001, 1, 7, 1, 176, 5082, 36, 50, 10, 13, 13, 17, '2020-12-16 15:38:16', '2020-12-16 15:38:16', '2020-12-16 15:39:54', b'1', '2020-12-16 15:40:49', NULL);
INSERT INTO `stock_trans` VALUES (8002, 1, 7, 1, 170, 5069, 28, 17, 9, 13, 13, 15, '2020-12-16 15:38:41', '2020-12-16 15:43:24', '2020-12-16 15:44:44', b'1', '2020-12-16 15:45:53', NULL);
INSERT INTO `stock_trans` VALUES (8003, 0, 7, 1, 178, 5165, 2, 33, 1, 11, 11, 19, '2020-12-16 15:40:26', '2020-12-16 15:42:54', '2020-12-16 15:45:14', b'1', '2020-12-16 15:45:15', NULL);
INSERT INTO `stock_trans` VALUES (8004, 1, 7, 1, 183, 5073, 28, 18, 9, 13, 13, 15, '2020-12-16 15:45:53', '2020-12-16 15:46:24', '2020-12-16 15:47:59', b'1', '2020-12-16 15:49:01', NULL);
INSERT INTO `stock_trans` VALUES (8005, 1, 7, 1, 178, 5076, 26, 17, 9, 13, 13, 19, '2020-12-16 15:49:11', '2020-12-16 15:51:26', '2020-12-16 15:52:34', b'1', '2020-12-16 15:53:35', NULL);
INSERT INTO `stock_trans` VALUES (8006, 1, 7, 1, 176, 5084, 36, 49, 10, 13, 13, 17, '2020-12-16 15:49:29', '2020-12-16 15:49:29', '2020-12-16 15:55:30', b'1', '2020-12-16 15:56:33', NULL);
INSERT INTO `stock_trans` VALUES (8007, 0, 7, 1, 178, 5166, 1, 33, 1, 11, 11, 15, '2020-12-16 15:52:03', '2020-12-16 15:54:30', '2020-12-16 15:56:22', b'1', '2020-12-16 15:56:23', NULL);
INSERT INTO `stock_trans` VALUES (8008, 0, 7, 1, 181, 5167, 15, 41, 8, 12, 12, 16, '2020-12-16 15:52:40', '2020-12-16 15:54:07', '2020-12-16 15:56:28', b'1', '2020-12-16 15:56:29', NULL);
INSERT INTO `stock_trans` VALUES (8009, 1, 7, 1, 178, 5080, 26, 18, 9, 13, 13, 19, '2020-12-16 15:53:38', '2020-12-16 15:54:05', '2020-12-16 15:58:16', b'1', '2020-12-16 15:59:12', NULL);
INSERT INTO `stock_trans` VALUES (8010, 1, 7, 1, 176, 5086, 36, 50, 10, 13, 13, 17, '2020-12-16 15:56:39', '2020-12-16 15:57:24', '2020-12-16 16:00:55', b'1', '2020-12-16 16:02:27', NULL);
INSERT INTO `stock_trans` VALUES (8011, 0, 7, 1, 178, 5168, 7, 22, 4, 11, 11, 15, '2020-12-16 16:01:56', '2020-12-16 16:03:28', '2020-12-16 16:06:11', b'1', '2020-12-16 16:06:12', NULL);
INSERT INTO `stock_trans` VALUES (8012, 1, 7, 1, 178, 5081, 26, 17, 9, 13, 13, 19, '2020-12-16 16:06:47', '2020-12-16 16:06:53', '2020-12-16 16:08:29', b'1', '2020-12-16 16:09:30', NULL);
INSERT INTO `stock_trans` VALUES (8013, 0, 7, 1, 178, 5169, 6, 22, 3, 11, 11, 15, '2020-12-16 16:06:48', '2020-12-16 16:09:11', '2020-12-16 16:11:34', b'1', '2020-12-16 16:11:35', NULL);
INSERT INTO `stock_trans` VALUES (8014, 0, 7, 1, 181, 5170, 9, 41, 5, 12, 12, 16, '2020-12-16 16:06:50', '2020-12-16 16:08:45', '2020-12-16 16:12:03', b'1', '2020-12-16 16:12:04', NULL);
INSERT INTO `stock_trans` VALUES (8015, 1, 7, 1, 176, 5087, 44, 49, 10, 13, 13, 17, '2020-12-16 16:06:50', '2020-12-16 16:06:54', '2020-12-16 16:12:05', b'1', '2020-12-16 16:13:31', NULL);
INSERT INTO `stock_trans` VALUES (8016, 0, 7, 1, 178, 5171, 2, 22, 1, 11, 11, 15, '2020-12-16 16:11:35', '2020-12-16 16:13:30', '2020-12-16 16:15:16', b'1', '2020-12-16 16:15:17', NULL);
INSERT INTO `stock_trans` VALUES (8017, 1, 7, 1, 176, 5089, 44, 50, 10, 13, 13, 17, '2020-12-16 16:13:34', '2020-12-16 16:14:03', '2020-12-16 16:19:40', b'1', '2020-12-16 16:20:59', NULL);
INSERT INTO `stock_trans` VALUES (8018, 1, 7, 1, 178, 5083, 26, 18, 9, 13, 13, 19, '2020-12-16 16:14:13', '2020-12-16 16:14:13', '2020-12-16 16:16:16', b'1', '2020-12-16 16:17:08', NULL);
INSERT INTO `stock_trans` VALUES (8019, 1, 7, 1, 178, 5085, 26, 17, 9, 13, 13, 19, '2020-12-16 16:22:28', '2020-12-16 16:22:28', '2020-12-16 16:24:40', b'1', '2020-12-16 16:25:47', NULL);
INSERT INTO `stock_trans` VALUES (8020, 0, 7, 1, 180, 5172, 11, 36, 6, 12, 12, 16, '2020-12-16 16:23:47', '2020-12-16 16:26:18', NULL, b'1', '2020-12-16 16:29:42', NULL);
INSERT INTO `stock_trans` VALUES (8021, 0, 7, 1, 184, 5173, 1, 26, 1, 11, 11, 15, '2020-12-16 16:26:25', '2020-12-16 16:27:53', '2020-12-16 16:30:20', b'1', '2020-12-16 16:30:21', NULL);
INSERT INTO `stock_trans` VALUES (8022, 1, 7, 1, 178, 5088, 24, 18, 9, 13, 13, 19, '2020-12-16 16:31:21', '2020-12-16 16:31:21', '2020-12-16 16:32:21', b'1', '2020-12-16 16:33:13', NULL);
INSERT INTO `stock_trans` VALUES (8023, 0, 7, 1, 184, 5174, 8, 26, 4, 11, 11, 15, '2020-12-16 16:35:52', '2020-12-16 16:38:05', '2020-12-16 16:40:26', b'1', '2020-12-16 16:40:27', NULL);
INSERT INTO `stock_trans` VALUES (8024, 1, 7, 1, 176, 5091, 44, 49, 10, 13, 13, 17, '2020-12-16 16:37:47', '2020-12-16 16:37:48', '2020-12-16 16:40:30', b'1', '2020-12-16 16:41:55', NULL);
INSERT INTO `stock_trans` VALUES (8025, 1, 7, 1, 178, 5090, 24, 17, 9, 13, 13, 19, '2020-12-16 16:39:49', '2020-12-16 16:39:50', '2020-12-16 16:44:06', b'1', '2020-12-16 16:45:00', NULL);
INSERT INTO `stock_trans` VALUES (8026, 0, 7, 1, 184, 5175, 5, 26, 3, 11, 11, 15, '2020-12-16 16:40:28', '2020-12-16 16:41:54', '2020-12-16 16:43:37', b'1', '2020-12-16 16:43:38', NULL);
INSERT INTO `stock_trans` VALUES (8027, 1, 7, 1, 176, 5093, 44, 50, 10, 13, 13, 17, '2020-12-16 16:41:57', '2020-12-16 16:42:37', '2020-12-16 16:47:36', b'1', '2020-12-16 16:48:53', NULL);
INSERT INTO `stock_trans` VALUES (8028, 0, 7, 1, 184, 5176, 2, 26, 1, 11, 11, 15, '2020-12-16 16:43:39', '2020-12-16 16:45:47', '2020-12-16 16:47:41', b'1', '2020-12-16 16:47:42', NULL);
INSERT INTO `stock_trans` VALUES (8029, 1, 7, 1, 178, 5092, 24, 18, 9, 13, 13, 19, '2020-12-16 16:47:51', '2020-12-16 16:47:51', '2020-12-16 16:51:02', b'1', '2020-12-16 16:51:56', NULL);
INSERT INTO `stock_trans` VALUES (8030, 1, 7, 1, 176, 5095, 44, 49, 10, 13, 13, 17, '2020-12-16 16:48:54', '2020-12-16 16:49:44', '2020-12-16 16:54:36', b'1', '2020-12-16 16:55:57', NULL);
INSERT INTO `stock_trans` VALUES (8031, 0, 7, 1, 184, 5177, 1, 26, 1, 11, 11, 15, '2020-12-16 16:51:30', '2020-12-16 16:53:08', '2020-12-16 16:54:30', b'1', '2020-12-16 16:54:31', NULL);
INSERT INTO `stock_trans` VALUES (8032, 1, 7, 1, 176, 5096, 37, 50, 10, 13, 13, 17, '2020-12-16 16:55:59', '2020-12-16 16:56:08', '2020-12-16 16:57:14', b'1', '2020-12-16 16:58:14', NULL);
INSERT INTO `stock_trans` VALUES (8033, 1, 7, 1, 178, 5094, 24, 17, 9, 13, 13, 19, '2020-12-16 16:57:02', '2020-12-16 16:57:02', '2020-12-16 17:00:03', b'1', '2020-12-16 17:00:57', NULL);
INSERT INTO `stock_trans` VALUES (8034, 0, 7, 1, 184, 5178, 7, 27, 4, 11, 11, 15, '2020-12-16 17:00:42', '2020-12-16 17:02:22', '2020-12-16 17:04:44', b'1', '2020-12-16 17:04:45', NULL);
INSERT INTO `stock_trans` VALUES (8035, 1, 7, 1, 176, 5098, 37, 49, 10, 13, 13, 17, '2020-12-16 17:04:41', '2020-12-16 17:04:41', '2020-12-16 17:06:24', b'1', '2020-12-16 17:07:30', NULL);
INSERT INTO `stock_trans` VALUES (8036, 0, 7, 1, 184, 5179, 2, 27, 1, 11, 11, 15, '2020-12-16 17:04:46', '2020-12-16 17:06:58', '2020-12-16 17:09:21', b'1', '2020-12-16 17:09:22', NULL);
INSERT INTO `stock_trans` VALUES (8037, 1, 7, 1, 178, 5097, 24, 18, 9, 13, 13, 19, '2020-12-16 17:05:48', '2020-12-16 17:05:48', '2020-12-16 17:09:17', b'1', '2020-12-16 17:10:37', NULL);
INSERT INTO `stock_trans` VALUES (8038, 0, 7, 1, 184, 5180, 6, 27, 3, 11, 11, 15, '2020-12-16 17:09:22', '2020-12-16 17:11:18', '2020-12-16 17:13:07', b'1', '2020-12-16 17:13:08', NULL);
INSERT INTO `stock_trans` VALUES (8039, 1, 7, 1, 178, 5099, 30, 17, 9, 13, 13, 19, '2020-12-16 17:14:00', '2020-12-16 17:14:00', '2020-12-16 17:15:21', b'1', '2020-12-16 17:16:35', NULL);
INSERT INTO `stock_trans` VALUES (8040, 0, 7, 1, 184, 5181, 1, 27, 1, 11, 11, 15, '2020-12-16 17:14:11', '2020-12-16 17:15:55', '2020-12-16 17:17:46', b'1', '2020-12-16 17:17:47', NULL);
INSERT INTO `stock_trans` VALUES (8041, 1, 7, 1, 176, 5100, 37, 50, 10, 13, 13, 17, '2020-12-16 17:15:08', '2020-12-16 17:15:08', '2020-12-16 17:18:13', b'1', '2020-12-16 17:19:10', NULL);
INSERT INTO `stock_trans` VALUES (8042, 1, 7, 1, 178, 5101, 30, 18, 9, 13, 13, 19, '2020-12-16 17:21:32', '2020-12-16 17:21:32', '2020-12-16 17:23:03', b'1', '2020-12-16 17:24:12', NULL);
INSERT INTO `stock_trans` VALUES (8043, 0, 7, 1, 184, 5174, 8, 27, 4, 11, 11, 15, '2020-12-16 17:23:21', '2020-12-16 17:25:33', '2020-12-16 17:27:05', b'1', '2020-12-16 17:27:06', NULL);
INSERT INTO `stock_trans` VALUES (8044, 1, 7, 1, 176, 5102, 37, 49, 10, 13, 13, 17, '2020-12-16 17:23:28', '2020-12-16 17:23:28', '2020-12-16 17:25:57', b'1', '2020-12-16 17:27:02', NULL);
INSERT INTO `stock_trans` VALUES (8045, 0, 7, 1, 184, 5182, 2, 24, 1, 11, 11, 15, '2020-12-16 17:27:07', '2020-12-16 17:29:21', '2020-12-16 17:31:47', b'1', '2020-12-16 17:31:48', NULL);
INSERT INTO `stock_trans` VALUES (8046, 1, 7, 1, 176, 5103, 37, 50, 10, 13, 13, 17, '2020-12-16 17:31:04', '2020-12-16 17:31:04', '2020-12-16 17:32:41', b'1', '2020-12-16 17:33:48', NULL);
INSERT INTO `stock_trans` VALUES (8047, 1, 7, 1, 178, 5104, 30, 17, 9, 13, 13, 19, '2020-12-16 17:31:20', '2020-12-16 17:31:21', '2020-12-16 17:35:34', b'1', '2020-12-16 17:36:48', NULL);
INSERT INTO `stock_trans` VALUES (8048, 0, 7, 1, 184, 5183, 5, 24, 3, 11, 11, 15, '2020-12-16 17:31:50', '2020-12-16 17:33:21', '2020-12-16 17:35:22', b'1', '2020-12-16 17:35:23', NULL);
INSERT INTO `stock_trans` VALUES (8049, 0, 7, 1, 184, 5184, 1, 24, 1, 11, 11, 15, '2020-12-16 17:37:18', '2020-12-16 17:38:49', '2020-12-16 17:40:38', b'1', '2020-12-16 17:40:39', NULL);
INSERT INTO `stock_trans` VALUES (8050, 1, 7, 1, 178, 5108, 30, 18, 9, 13, 13, 19, '2020-12-16 17:39:25', '2020-12-16 17:39:25', '2020-12-16 17:40:42', b'1', '2020-12-16 17:41:48', NULL);
INSERT INTO `stock_trans` VALUES (8051, 1, 7, 1, 176, 5105, 35, 49, 10, 13, 13, 17, '2020-12-16 17:42:19', '2020-12-16 17:42:20', '2020-12-16 17:43:53', b'1', '2020-12-16 17:44:55', NULL);
INSERT INTO `stock_trans` VALUES (8052, 1, 7, 1, 178, 5109, 30, 17, 9, 13, 13, 19, '2020-12-16 17:47:40', '2020-12-16 17:47:40', '2020-12-16 17:49:16', b'1', '2020-12-16 17:50:45', NULL);
INSERT INTO `stock_trans` VALUES (8053, 1, 7, 1, 176, 5106, 35, 50, 10, 13, 13, 17, '2020-12-16 17:49:45', '2020-12-16 17:49:46', '2020-12-16 17:52:09', b'1', '2020-12-16 17:53:05', NULL);
INSERT INTO `stock_trans` VALUES (8054, 0, 7, 1, 184, 5185, 7, 24, 4, 11, 11, 15, '2020-12-16 17:49:50', '2020-12-16 17:51:36', '2020-12-16 17:53:26', b'1', '2020-12-16 17:53:27', NULL);
INSERT INTO `stock_trans` VALUES (8055, 1, 7, 1, 178, 5110, 31, 18, 9, 13, 13, 19, '2020-12-16 17:56:54', '2020-12-16 17:57:03', '2020-12-16 17:58:43', b'1', '2020-12-16 17:59:53', NULL);
INSERT INTO `stock_trans` VALUES (8056, 0, 7, 1, 184, 5186, 6, 24, 3, 11, 11, 15, '2020-12-16 17:56:58', '2020-12-16 17:59:09', '2020-12-16 18:00:36', b'1', '2020-12-16 18:00:37', NULL);
INSERT INTO `stock_trans` VALUES (8057, 1, 7, 1, 176, 5107, 35, 49, 10, 13, 13, 17, '2020-12-16 17:57:42', '2020-12-16 17:57:42', '2020-12-16 18:01:21', b'1', '2020-12-16 18:02:23', NULL);
INSERT INTO `stock_trans` VALUES (8058, 0, 7, 1, 184, 5187, 2, 28, 1, 11, 11, 15, '2020-12-16 18:00:38', '2020-12-16 18:02:41', '2020-12-16 18:05:18', b'1', '2020-12-16 18:05:19', NULL);
INSERT INTO `stock_trans` VALUES (8059, 1, 7, 1, 178, 5113, 31, 17, 9, 13, 13, 19, '2020-12-16 18:04:19', '2020-12-16 18:04:20', '2020-12-16 18:05:42', b'1', '2020-12-16 18:06:56', NULL);
INSERT INTO `stock_trans` VALUES (8060, 0, 7, 1, 184, 5188, 1, 28, 1, 11, 11, 15, '2020-12-16 18:05:21', '2020-12-16 18:07:03', '2020-12-16 18:09:11', b'1', '2020-12-16 18:09:12', NULL);
INSERT INTO `stock_trans` VALUES (8061, 1, 7, 1, 176, 5111, 35, 50, 10, 13, 13, 17, '2020-12-16 18:05:57', '2020-12-16 18:05:57', '2020-12-16 18:08:20', b'1', '2020-12-16 18:09:12', NULL);
INSERT INTO `stock_trans` VALUES (8062, 1, 7, 1, 176, 5112, 35, 49, 10, 13, 13, 17, '2020-12-16 18:13:03', '2020-12-16 18:13:03', '2020-12-16 18:14:33', b'1', '2020-12-16 18:16:11', NULL);
INSERT INTO `stock_trans` VALUES (8063, 1, 7, 1, 178, 5116, 31, 18, 9, 13, 13, 19, '2020-12-16 18:13:50', '2020-12-16 18:13:51', '2020-12-16 18:18:08', b'1', '2020-12-16 18:19:17', NULL);
INSERT INTO `stock_trans` VALUES (8064, 1, 7, 1, 176, 5114, 43, 50, 10, 13, 13, 17, '2020-12-16 18:21:01', '2020-12-16 18:21:01', '2020-12-16 18:23:06', b'1', '2020-12-16 18:24:21', NULL);
INSERT INTO `stock_trans` VALUES (8065, 1, 7, 1, 178, 5118, 31, 17, 9, 13, 13, 19, '2020-12-16 18:21:32', '2020-12-16 18:21:33', '2020-12-16 18:26:26', b'1', '2020-12-16 18:27:41', NULL);
INSERT INTO `stock_trans` VALUES (8066, 1, 7, 1, 176, 5115, 43, 49, 10, 13, 13, 17, '2020-12-16 18:30:52', '2020-12-16 18:30:52', '2020-12-16 18:33:04', b'1', '2020-12-16 18:34:25', NULL);
INSERT INTO `stock_trans` VALUES (8067, 1, 7, 1, 178, 5120, 31, 18, 9, 13, 13, 19, '2020-12-16 18:31:50', '2020-12-16 18:31:50', '2020-12-16 18:36:25', b'1', '2020-12-16 18:37:39', NULL);
INSERT INTO `stock_trans` VALUES (8068, 0, 7, 1, 181, 5189, 15, 41, 8, 12, 12, 16, '2020-12-16 18:34:43', '2020-12-16 18:36:25', NULL, b'1', '2020-12-16 18:39:33', NULL);
INSERT INTO `stock_trans` VALUES (8069, 1, 7, 1, 176, 5117, 43, 50, 10, 13, 13, 17, '2020-12-16 18:38:40', '2020-12-16 18:38:40', '2020-12-16 18:40:44', b'1', '2020-12-16 18:41:59', NULL);
INSERT INTO `stock_trans` VALUES (8070, 1, 7, 1, 178, 5122, 19, 17, 9, 13, 13, 19, '2020-12-16 18:40:12', '2020-12-16 18:40:12', '2020-12-16 18:44:20', b'1', '2020-12-16 18:45:20', NULL);
INSERT INTO `stock_trans` VALUES (8071, 1, 7, 1, 178, 5125, 19, 18, 9, 13, 13, 19, '2020-12-16 18:50:38', '2020-12-16 18:50:39', '2020-12-16 18:52:37', b'1', '2020-12-16 18:53:41', NULL);
INSERT INTO `stock_trans` VALUES (8072, 1, 7, 1, 176, 5119, 43, 49, 10, 13, 13, 17, '2020-12-16 18:54:33', '2020-12-16 18:54:34', '2020-12-16 18:56:55', b'1', '2020-12-16 18:58:16', NULL);
INSERT INTO `stock_trans` VALUES (8073, 1, 7, 1, 178, 5129, 19, 17, 9, 13, 13, 19, '2020-12-16 19:01:59', '2020-12-16 19:02:02', '2020-12-16 19:04:24', b'1', '2020-12-16 19:05:22', NULL);
INSERT INTO `stock_trans` VALUES (8074, 1, 7, 1, 176, 5121, 43, 50, 10, 13, 13, 17, '2020-12-16 19:06:52', '2020-12-16 19:06:53', '2020-12-16 19:08:55', b'1', '2020-12-16 19:10:14', NULL);
INSERT INTO `stock_trans` VALUES (8075, 1, 7, 1, 178, 5132, 19, 18, 9, 13, 13, 19, '2020-12-16 19:08:38', '2020-12-16 19:08:39', '2020-12-16 19:12:34', b'1', '2020-12-16 19:13:36', NULL);
INSERT INTO `stock_trans` VALUES (8076, 1, 7, 1, 176, 5123, 40, 49, 10, 13, 13, 17, '2020-12-16 19:15:40', '2020-12-16 19:15:40', '2020-12-16 19:17:33', b'1', '2020-12-16 19:18:45', NULL);
INSERT INTO `stock_trans` VALUES (8077, 1, 7, 1, 178, 5133, 19, 17, 9, 13, 13, 19, '2020-12-16 19:17:29', '2020-12-16 19:17:29', '2020-12-16 19:20:58', b'1', '2020-12-16 19:22:29', NULL);
INSERT INTO `stock_trans` VALUES (8078, 1, 7, 1, 176, 5126, 40, 50, 10, 13, 13, 17, '2020-12-16 19:23:05', '2020-12-16 19:23:05', '2020-12-16 19:24:50', b'1', '2020-12-16 19:25:57', NULL);
INSERT INTO `stock_trans` VALUES (8079, 1, 7, 1, 178, 5135, 32, 18, 9, 13, 13, 19, '2020-12-16 19:26:20', '2020-12-16 19:26:20', '2020-12-16 19:29:01', b'1', '2020-12-16 19:30:11', NULL);
INSERT INTO `stock_trans` VALUES (8080, 1, 7, 1, 176, 5128, 40, 49, 10, 13, 13, 17, '2020-12-16 19:30:33', '2020-12-16 19:30:33', '2020-12-16 19:32:24', b'1', '2020-12-16 19:33:37', NULL);
INSERT INTO `stock_trans` VALUES (8081, 1, 7, 1, 178, 5137, 32, 17, 9, 13, 13, 19, '2020-12-16 19:34:10', '2020-12-16 19:34:11', '2020-12-16 19:36:20', b'1', '2020-12-16 19:37:37', NULL);
INSERT INTO `stock_trans` VALUES (8082, 1, 7, 1, 178, 5139, 32, 18, 9, 13, 13, 19, '2020-12-16 19:41:51', '2020-12-16 19:41:52', '2020-12-16 19:44:51', b'1', '2020-12-16 19:46:01', NULL);
INSERT INTO `stock_trans` VALUES (8083, 1, 7, 1, 176, 5130, 40, 50, 10, 13, 13, 17, '2020-12-16 19:42:32', '2020-12-16 19:42:33', '2020-12-16 19:47:42', b'1', '2020-12-16 19:48:50', NULL);
INSERT INTO `stock_trans` VALUES (8084, 1, 7, 1, 176, 5131, 40, 49, 10, 13, 13, 17, '2020-12-16 19:50:17', '2020-12-16 19:50:18', '2020-12-16 19:55:21', b'1', '2020-12-16 19:56:31', NULL);
INSERT INTO `stock_trans` VALUES (8085, 1, 7, 1, 178, 5140, 32, 17, 9, 13, 13, 19, '2020-12-16 19:50:30', '2020-12-16 19:50:31', '2020-12-16 19:52:09', b'1', '2020-12-16 19:53:26', NULL);
INSERT INTO `stock_trans` VALUES (8086, 1, 7, 1, 178, 5142, 32, 18, 9, 13, 13, 19, '2020-12-16 19:58:09', '2020-12-16 19:58:10', '2020-12-16 20:08:13', b'1', '2020-12-16 20:09:12', NULL);
INSERT INTO `stock_trans` VALUES (8087, 0, 7, 1, 185, 5190, 4, 19, 2, 11, 11, 15, '2020-12-16 19:59:32', '2020-12-16 20:01:35', '2020-12-16 20:04:09', b'1', '2020-12-16 20:04:10', NULL);
INSERT INTO `stock_trans` VALUES (8088, 0, 7, 1, 184, 5191, 5, 28, 3, 11, 11, 18, '2020-12-16 20:03:44', '2020-12-16 20:06:49', '2020-12-16 20:08:39', b'1', '2020-12-16 20:08:40', NULL);
INSERT INTO `stock_trans` VALUES (8089, 1, 7, 1, 176, 5134, 34, 50, 10, 13, 13, 17, '2020-12-16 20:11:20', '2020-12-16 20:11:20', '2020-12-16 20:12:47', b'1', '2020-12-16 20:13:40', NULL);
INSERT INTO `stock_trans` VALUES (8090, 0, 7, 1, 184, 5192, 8, 28, 4, 11, 11, 18, '2020-12-16 20:13:44', '2020-12-16 20:15:50', '2020-12-16 20:17:48', b'1', '2020-12-16 20:17:49', NULL);
INSERT INTO `stock_trans` VALUES (8091, 1, 7, 1, 178, 5145, 25, 17, 9, 13, 13, 19, '2020-12-16 20:16:22', '2020-12-16 20:16:23', '2020-12-16 20:17:56', b'1', '2020-12-16 20:18:52', NULL);
INSERT INTO `stock_trans` VALUES (8092, 1, 7, 1, 176, 5136, 34, 49, 10, 13, 13, 17, '2020-12-16 20:21:31', '2020-12-16 20:21:31', '2020-12-16 20:22:55', b'1', '2020-12-16 20:23:52', NULL);
INSERT INTO `stock_trans` VALUES (8093, 1, 7, 1, 178, 5146, 25, 18, 9, 13, 13, 19, '2020-12-16 20:25:39', '2020-12-16 20:25:39', '2020-12-16 20:27:07', b'1', '2020-12-16 20:27:55', NULL);
INSERT INTO `stock_trans` VALUES (8094, 1, 7, 1, 176, 5138, 34, 50, 10, 13, 13, 17, '2020-12-16 20:31:03', '2020-12-16 20:31:04', '2020-12-16 20:32:28', b'1', '2020-12-16 20:33:20', NULL);
INSERT INTO `stock_trans` VALUES (8095, 1, 7, 1, 178, 5147, 25, 17, 9, 13, 13, 19, '2020-12-16 20:34:46', '2020-12-16 20:34:46', '2020-12-16 20:36:20', b'1', '2020-12-16 20:37:17', NULL);
INSERT INTO `stock_trans` VALUES (8096, 1, 7, 1, 176, 5141, 34, 49, 10, 13, 13, 17, '2020-12-16 20:42:32', '2020-12-16 20:42:33', '2020-12-16 20:44:00', b'1', '2020-12-16 20:44:57', NULL);
INSERT INTO `stock_trans` VALUES (8097, 1, 7, 1, 178, 5149, 25, 18, 9, 13, 13, 19, '2020-12-16 20:46:39', '2020-12-16 20:46:40', '2020-12-16 20:48:08', b'1', '2020-12-16 20:48:55', NULL);
INSERT INTO `stock_trans` VALUES (8098, 1, 7, 1, 178, 5151, 25, 17, 9, 13, 13, 19, '2020-12-16 20:57:52', '2020-12-16 20:57:52', '2020-12-16 20:59:28', b'1', '2020-12-16 21:00:34', NULL);
INSERT INTO `stock_trans` VALUES (8099, 1, 7, 1, 180, 5148, 42, 50, 10, 13, 13, 15, '2020-12-16 20:58:44', '2020-12-16 21:03:07', '2020-12-16 21:04:26', b'1', '2020-12-16 21:05:40', NULL);
INSERT INTO `stock_trans` VALUES (8100, 1, 7, 1, 180, 5150, 42, 49, 10, 13, NULL, 15, '2020-12-16 21:05:42', NULL, NULL, b'1', '2020-12-16 21:06:42', NULL);
INSERT INTO `stock_trans` VALUES (8101, 1, 7, 1, 178, 5152, 21, 18, 9, 13, 13, 19, '2020-12-16 21:11:32', '2020-12-16 21:11:33', '2020-12-16 21:13:23', b'1', '2020-12-16 21:14:24', NULL);
INSERT INTO `stock_trans` VALUES (8102, 1, 7, 1, 180, 5150, 42, 49, 10, 13, 13, 15, '2020-12-16 21:16:02', '2020-12-16 21:16:03', '2020-12-16 21:18:11', b'1', '2020-12-16 21:19:30', NULL);
INSERT INTO `stock_trans` VALUES (8103, 1, 7, 1, 180, 5154, 42, 50, 10, 13, 13, 15, '2020-12-16 21:24:13', '2020-12-16 21:24:13', '2020-12-16 21:26:21', b'1', '2020-12-16 21:27:46', NULL);
INSERT INTO `stock_trans` VALUES (8104, 1, 7, 1, 178, 5153, 21, 17, 9, 13, 13, 19, '2020-12-16 21:28:03', '2020-12-16 21:28:03', '2020-12-16 21:30:13', b'1', '2020-12-16 21:31:06', NULL);
INSERT INTO `stock_trans` VALUES (8105, 1, 7, 1, 177, 5124, 39, 49, 10, 13, 13, 18, '2020-12-16 21:31:24', '2020-12-16 21:34:19', '2020-12-16 21:35:39', b'1', '2020-12-16 21:37:02', NULL);
INSERT INTO `stock_trans` VALUES (8106, 1, 7, 1, 178, 5157, 21, 18, 9, 13, 13, 19, '2020-12-16 21:37:18', '2020-12-16 21:37:19', '2020-12-16 21:40:09', b'1', '2020-12-16 21:41:11', NULL);
INSERT INTO `stock_trans` VALUES (8107, 1, 7, 1, 178, 5158, 21, 17, 9, 13, 13, 19, '2020-12-16 21:49:31', '2020-12-16 21:49:31', '2020-12-16 21:50:34', b'1', '2020-12-16 21:51:28', NULL);
INSERT INTO `stock_trans` VALUES (8108, 1, 7, 1, 177, 5155, 38, 49, 10, 13, 13, 17, '2020-12-16 21:54:03', '2020-12-16 21:56:21', '2020-12-16 21:57:38', b'1', '2020-12-16 21:58:58', NULL);
INSERT INTO `stock_trans` VALUES (8109, 1, 7, 1, 178, 5159, 21, 18, 9, 13, 13, 19, '2020-12-16 22:01:33', '2020-12-16 22:01:33', '2020-12-16 22:03:42', b'1', '2020-12-16 22:05:09', NULL);
INSERT INTO `stock_trans` VALUES (8110, 1, 7, 1, 181, 5167, 41, 50, 10, 13, 13, 16, '2020-12-16 22:03:47', '2020-12-16 22:06:46', '2020-12-16 22:08:06', b'1', '2020-12-16 22:09:31', NULL);
INSERT INTO `stock_trans` VALUES (8111, 1, 7, 1, 175, 4929, 23, 49, 10, 13, 13, 17, '2020-12-16 22:11:07', '2020-12-16 22:13:36', '2020-12-16 22:15:05', b'1', '2020-12-16 22:16:25', NULL);
INSERT INTO `stock_trans` VALUES (8112, 1, 7, 1, 178, 5160, 33, 17, 9, 13, 13, 19, '2020-12-16 22:15:48', '2020-12-16 22:15:49', '2020-12-16 22:18:34', b'1', '2020-12-16 22:19:55', NULL);
INSERT INTO `stock_trans` VALUES (8113, 1, 7, 1, 175, 4968, 23, 50, 10, 13, 13, 17, '2020-12-16 22:28:26', '2020-12-16 22:28:27', '2020-12-16 22:45:29', b'1', '2020-12-16 22:46:56', b'1');
INSERT INTO `stock_trans` VALUES (8114, 1, 7, 1, 178, 5161, 33, 18, 9, 13, 13, 19, '2020-12-16 22:31:44', '2020-12-16 22:31:45', '2020-12-16 22:49:01', b'1', '2020-12-16 22:50:16', NULL);
INSERT INTO `stock_trans` VALUES (8115, 1, 7, 1, 178, 5163, 33, 17, 9, 13, 13, 19, '2020-12-16 22:50:17', '2020-12-16 22:50:49', '2020-12-16 22:52:47', b'1', '2020-12-16 22:54:10', NULL);
INSERT INTO `stock_trans` VALUES (8116, 1, 7, 1, 175, 5000, 23, 49, 10, 13, 13, 17, '2020-12-16 22:55:52', '2020-12-16 22:55:53', '2020-12-16 22:57:59', b'1', '2020-12-16 22:59:19', NULL);
INSERT INTO `stock_trans` VALUES (8117, 1, 7, 1, 178, 5165, 33, 18, 9, 13, 13, 19, '2020-12-16 22:56:42', '2020-12-16 22:56:43', '2020-12-16 23:01:22', b'1', '2020-12-16 23:02:36', NULL);
INSERT INTO `stock_trans` VALUES (8118, 1, 7, 1, 175, 5034, 23, 50, 10, 13, 13, 17, '2020-12-16 23:04:40', '2020-12-16 23:04:40', '2020-12-16 23:06:54', b'1', '2020-12-16 23:08:16', NULL);
INSERT INTO `stock_trans` VALUES (8119, 1, 7, 1, 178, 5166, 33, 17, 9, 13, 13, 19, '2020-12-16 23:07:44', '2020-12-16 23:07:45', '2020-12-16 23:10:07', b'1', '2020-12-16 23:11:13', NULL);
INSERT INTO `stock_trans` VALUES (8120, 1, 7, 1, 175, 5070, 29, 49, 10, 13, 13, 17, '2020-12-16 23:11:59', '2020-12-16 23:12:00', '2020-12-16 23:13:30', b'1', '2020-12-16 23:14:47', NULL);
INSERT INTO `stock_trans` VALUES (8121, 1, 7, 1, 178, 5168, 22, 18, 9, 13, 13, 19, '2020-12-16 23:17:30', '2020-12-16 23:17:31', '2020-12-16 23:19:52', b'1', '2020-12-16 23:20:50', NULL);
INSERT INTO `stock_trans` VALUES (8122, 1, 7, 1, 178, 5169, 22, 17, 9, 13, 13, 19, '2020-12-16 23:28:55', '2020-12-16 23:28:55', '2020-12-16 23:30:40', b'1', '2020-12-16 23:31:31', NULL);
INSERT INTO `stock_trans` VALUES (8123, 0, 7, 1, 184, 5193, 2, 21, 1, 11, 11, 17, '2020-12-16 23:38:35', '2020-12-16 23:41:42', '2020-12-16 23:44:13', b'1', '2020-12-16 23:44:14', NULL);
INSERT INTO `stock_trans` VALUES (8124, 1, 7, 1, 178, 5171, 22, 18, 9, 13, 13, 19, '2020-12-16 23:39:28', '2020-12-16 23:39:28', '2020-12-16 23:41:26', b'1', '2020-12-16 23:42:35', NULL);
INSERT INTO `stock_trans` VALUES (8125, 1, 7, 1, 184, 5173, 26, 17, 9, 13, 13, 17, '2020-12-17 00:00:20', '2020-12-17 00:02:49', '2020-12-17 00:04:05', b'1', '2020-12-17 00:05:08', NULL);
INSERT INTO `stock_trans` VALUES (8126, 0, 7, 1, 184, 5194, 1, 21, 1, 11, 11, 19, '2020-12-17 00:07:04', '2020-12-17 00:09:03', '2020-12-17 00:10:58', b'1', '2020-12-17 00:10:59', NULL);
INSERT INTO `stock_trans` VALUES (8127, 1, 7, 1, 184, 5175, 26, 18, 9, 13, 13, 17, '2020-12-17 00:08:45', '2020-12-17 00:08:46', '2020-12-17 00:10:03', b'1', '2020-12-17 00:10:59', NULL);
INSERT INTO `stock_trans` VALUES (8128, 0, 7, 1, 184, 5195, 7, 21, 4, 11, 11, 19, '2020-12-17 00:17:50', '2020-12-17 00:19:43', '2020-12-17 00:22:03', b'1', '2020-12-17 00:22:04', NULL);
INSERT INTO `stock_trans` VALUES (8129, 0, 7, 1, 184, 5196, 6, 21, 3, 11, 11, 19, '2020-12-17 00:22:05', '2020-12-17 00:24:20', '2020-12-17 00:26:26', b'1', '2020-12-17 00:26:27', NULL);
INSERT INTO `stock_trans` VALUES (8130, 1, 7, 1, 184, 5176, 26, 17, 9, 13, 13, 17, '2020-12-17 00:22:41', '2020-12-17 00:22:41', '2020-12-17 00:23:58', b'1', '2020-12-17 00:25:05', NULL);
INSERT INTO `stock_trans` VALUES (8131, 0, 7, 1, 184, 5197, 2, 21, 1, 11, 11, 19, '2020-12-17 00:27:43', '2020-12-17 00:29:33', '2020-12-17 00:30:54', b'1', '2020-12-17 00:30:55', NULL);
INSERT INTO `stock_trans` VALUES (8132, 1, 7, 1, 184, 5177, 26, 18, 9, 13, 13, 17, '2020-12-17 00:30:52', '2020-12-17 00:30:52', '2020-12-17 00:32:03', b'1', '2020-12-17 00:33:19', NULL);
INSERT INTO `stock_trans` VALUES (8133, 1, 7, 1, 184, 5178, 27, 17, 9, 13, 13, 17, '2020-12-17 00:38:49', '2020-12-17 00:38:49', '2020-12-17 00:40:08', b'1', '2020-12-17 00:41:15', NULL);
INSERT INTO `stock_trans` VALUES (8134, 0, 7, 1, 184, 5198, 1, 22, 1, 11, 11, 19, '2020-12-17 00:39:33', '2020-12-17 00:40:54', '2020-12-17 00:43:08', b'1', '2020-12-17 00:43:09', NULL);
INSERT INTO `stock_trans` VALUES (8135, 0, 7, 1, 184, 5199, 8, 22, 4, 11, 11, 19, '2020-12-17 00:43:10', '2020-12-17 00:45:35', '2020-12-17 00:48:12', b'1', '2020-12-17 00:48:13', NULL);
INSERT INTO `stock_trans` VALUES (8136, 1, 7, 1, 184, 5179, 27, 18, 9, 13, 13, 17, '2020-12-17 00:46:58', '2020-12-17 00:46:59', '2020-12-17 00:53:38', b'1', '2020-12-17 00:54:37', NULL);
INSERT INTO `stock_trans` VALUES (8137, 0, 7, 1, 184, 5200, 5, 22, 3, 11, 11, 19, '2020-12-17 00:48:16', '2020-12-17 00:49:52', '2020-12-17 00:51:54', b'1', '2020-12-17 00:51:55', NULL);
INSERT INTO `stock_trans` VALUES (8138, 0, 7, 1, 184, 5201, 2, 22, 1, 11, 11, 19, '2020-12-17 00:51:56', '2020-12-17 00:53:44', '2020-12-17 00:55:26', b'1', '2020-12-17 00:55:27', NULL);
INSERT INTO `stock_trans` VALUES (8139, 1, 7, 1, 184, 5180, 27, 17, 9, 13, 13, 17, '2020-12-17 00:56:40', '2020-12-17 00:56:41', '2020-12-17 00:58:33', b'1', '2020-12-17 00:59:39', NULL);
INSERT INTO `stock_trans` VALUES (8140, 1, 7, 1, 184, 5181, 27, 18, 9, 13, 13, 17, '2020-12-17 01:06:03', '2020-12-17 01:06:04', '2020-12-17 01:11:51', b'1', '2020-12-17 01:12:53', NULL);
INSERT INTO `stock_trans` VALUES (8141, 0, 7, 1, 184, 5202, 1, 22, 1, 11, 11, 19, '2020-12-17 01:11:19', '2020-12-17 01:12:42', '2020-12-17 01:13:57', b'1', '2020-12-17 01:13:58', NULL);
INSERT INTO `stock_trans` VALUES (8142, 0, 7, 1, 186, 5203, 15, 44, 8, 12, 12, 15, '2020-12-17 01:12:53', '2020-12-17 01:14:59', '2020-12-17 01:17:28', b'1', '2020-12-17 01:17:29', NULL);
INSERT INTO `stock_trans` VALUES (8143, 0, 7, 1, 184, 5204, 7, 23, 4, 11, 11, 19, '2020-12-17 01:14:11', '2020-12-17 01:16:01', '2020-12-17 01:18:38', b'1', '2020-12-17 01:18:39', NULL);
INSERT INTO `stock_trans` VALUES (8144, 1, 7, 1, 184, 5174, 27, 17, 9, 13, 13, 17, '2020-12-17 01:15:21', '2020-12-17 01:15:21', '2020-12-17 01:16:41', b'1', '2020-12-17 01:17:57', NULL);
INSERT INTO `stock_trans` VALUES (8145, 0, 7, 1, 184, 5205, 6, 23, 3, 11, 11, 19, '2020-12-17 01:18:40', '2020-12-17 01:20:46', '2020-12-17 01:23:06', b'1', '2020-12-17 01:23:07', NULL);
INSERT INTO `stock_trans` VALUES (8146, 0, 7, 1, 185, 5206, 3, 25, 2, 11, 11, 18, '2020-12-17 01:19:06', '2020-12-17 01:25:16', '2020-12-17 01:27:42', b'1', '2020-12-17 01:27:43', NULL);
INSERT INTO `stock_trans` VALUES (8147, 0, 7, 1, 186, 5207, 16, 42, 8, 12, 12, 15, '2020-12-17 01:20:50', '2020-12-17 01:22:52', '2020-12-17 01:25:28', b'1', '2020-12-17 01:25:29', NULL);
INSERT INTO `stock_trans` VALUES (8148, 0, 7, 1, 184, 5208, 2, 23, 1, 11, 11, 19, '2020-12-17 01:23:45', '2020-12-17 01:28:16', '2020-12-17 01:30:13', b'1', '2020-12-17 01:30:14', NULL);
INSERT INTO `stock_trans` VALUES (8149, 0, 7, 1, 186, 5209, 10, 42, 5, 12, 12, 15, '2020-12-17 01:25:29', '2020-12-17 01:28:00', '2020-12-17 01:30:39', b'1', '2020-12-17 01:30:40', NULL);
INSERT INTO `stock_trans` VALUES (8150, 1, 7, 1, 184, 5182, 24, 18, 9, 13, 13, 17, '2020-12-17 01:26:18', '2020-12-17 01:26:19', '2020-12-17 01:32:59', b'1', '2020-12-17 01:33:58', NULL);
INSERT INTO `stock_trans` VALUES (8151, 0, 7, 1, 186, 5211, 11, 42, 6, 12, 12, 15, '2020-12-17 01:30:43', '2020-12-17 01:32:26', '2020-12-17 01:34:33', b'1', '2020-12-17 01:34:34', NULL);
INSERT INTO `stock_trans` VALUES (8152, 1, 7, 1, 184, 5183, 24, 17, 9, 13, 13, 17, '2020-12-17 01:36:15', '2020-12-17 01:36:16', '2020-12-17 01:37:24', b'1', '2020-12-17 01:38:22', NULL);
INSERT INTO `stock_trans` VALUES (8153, 0, 7, 1, 186, 5212, 15, 40, 8, 12, 12, 15, '2020-12-17 01:38:09', '2020-12-17 01:39:35', '2020-12-17 01:42:05', b'1', '2020-12-17 01:42:06', NULL);
INSERT INTO `stock_trans` VALUES (8154, 0, 7, 1, 184, 5213, 1, 23, 1, 11, 11, 19, '2020-12-17 01:38:28', '2020-12-17 01:39:57', '2020-12-17 01:41:35', b'1', '2020-12-17 01:41:36', NULL);
INSERT INTO `stock_trans` VALUES (8155, 0, 7, 1, 184, 5214, 8, 23, 4, 11, 11, 19, '2020-12-17 01:41:37', '2020-12-17 01:43:53', '2020-12-17 01:45:45', b'1', '2020-12-17 01:45:46', NULL);
INSERT INTO `stock_trans` VALUES (8156, 0, 7, 1, 186, 5215, 12, 40, 6, 12, 12, 15, '2020-12-17 01:45:47', '2020-12-17 01:48:01', '2020-12-17 01:50:27', b'1', '2020-12-17 01:50:28', NULL);
INSERT INTO `stock_trans` VALUES (8157, 0, 7, 1, 184, 5216, 5, 27, 3, 11, 11, 19, '2020-12-17 01:45:48', '2020-12-17 01:47:25', '2020-12-17 01:49:27', b'1', '2020-12-17 01:49:28', NULL);
INSERT INTO `stock_trans` VALUES (8158, 1, 7, 1, 184, 5184, 24, 18, 9, 13, 13, 17, '2020-12-17 01:45:55', '2020-12-17 01:45:55', '2020-12-17 01:47:11', b'1', '2020-12-17 01:48:06', NULL);
INSERT INTO `stock_trans` VALUES (8159, 0, 7, 1, 184, 5217, 2, 27, 1, 11, 11, 19, '2020-12-17 01:50:13', '2020-12-17 01:52:19', '2020-12-17 01:54:41', b'1', '2020-12-17 01:54:42', NULL);
INSERT INTO `stock_trans` VALUES (8160, 1, 7, 1, 184, 5185, 24, 17, 9, 13, 13, 17, '2020-12-17 01:55:47', '2020-12-17 01:55:48', '2020-12-17 01:56:59', b'1', '2020-12-17 01:57:56', NULL);
INSERT INTO `stock_trans` VALUES (8161, 0, 7, 1, 186, 5218, 16, 38, 8, 12, 12, 15, '2020-12-17 02:00:38', '2020-12-17 02:02:48', '2020-12-17 02:05:39', b'1', '2020-12-17 02:05:40', NULL);
INSERT INTO `stock_trans` VALUES (8162, 0, 7, 1, 187, 5219, 14, 37, 7, 12, 12, 16, '2020-12-17 02:04:03', '2020-12-17 02:06:46', '2020-12-17 02:09:30', b'1', '2020-12-17 02:09:31', NULL);
INSERT INTO `stock_trans` VALUES (8163, 0, 7, 1, 184, 5220, 1, 27, 1, 11, 11, 19, '2020-12-17 02:04:50', '2020-12-17 02:06:28', '2020-12-17 02:08:11', b'1', '2020-12-17 02:08:12', NULL);
INSERT INTO `stock_trans` VALUES (8164, 1, 7, 1, 184, 5186, 24, 18, 9, 13, 13, 17, '2020-12-17 02:05:07', '2020-12-17 02:05:07', '2020-12-17 02:14:32', b'1', '2020-12-17 02:15:48', NULL);
INSERT INTO `stock_trans` VALUES (8165, 0, 7, 1, 184, 5221, 7, 29, 4, 11, 11, 18, '2020-12-17 02:10:08', '2020-12-17 02:12:11', '2020-12-17 02:14:34', b'1', '2020-12-17 02:14:35', NULL);
INSERT INTO `stock_trans` VALUES (8166, 0, 7, 1, 186, 5222, 11, 38, 6, 12, 12, 15, '2020-12-17 02:10:16', '2020-12-17 02:12:05', '2020-12-17 02:14:14', b'1', '2020-12-17 02:14:15', NULL);
INSERT INTO `stock_trans` VALUES (8167, 0, 7, 1, 186, 5223, 9, 38, 5, 12, 12, 15, '2020-12-17 02:14:17', '2020-12-17 02:15:55', '2020-12-17 02:17:57', b'1', '2020-12-17 02:17:58', NULL);
INSERT INTO `stock_trans` VALUES (8168, 0, 7, 1, 184, 5224, 6, 29, 3, 11, 11, 18, '2020-12-17 02:14:38', '2020-12-17 02:16:44', '2020-12-17 02:19:04', b'1', '2020-12-17 02:19:05', NULL);
INSERT INTO `stock_trans` VALUES (8169, 0, 7, 1, 187, 5225, 13, 37, 7, 12, 12, 16, '2020-12-17 02:14:42', '2020-12-17 02:18:50', '2020-12-17 02:21:04', b'1', '2020-12-17 02:21:05', NULL);
INSERT INTO `stock_trans` VALUES (8170, 1, 7, 1, 184, 5187, 28, 17, 9, 13, 13, 17, '2020-12-17 02:15:49', '2020-12-17 02:16:02', '2020-12-17 02:17:22', b'1', '2020-12-17 02:18:33', NULL);
INSERT INTO `stock_trans` VALUES (8171, 0, 7, 1, 186, 5226, 15, 38, 8, 12, 12, 15, '2020-12-17 02:18:01', '2020-12-17 02:21:40', '2020-12-17 02:23:32', b'1', '2020-12-17 02:23:33', NULL);
INSERT INTO `stock_trans` VALUES (8172, 1, 7, 1, 184, 5188, 28, 18, 9, 13, 13, 17, '2020-12-17 02:23:31', '2020-12-17 02:23:32', '2020-12-17 02:27:32', b'1', '2020-12-17 02:28:34', NULL);
INSERT INTO `stock_trans` VALUES (8173, 1, 7, 1, 184, 5191, 28, 17, 9, 13, 13, 17, '2020-12-17 02:32:26', '2020-12-17 02:32:26', '2020-12-17 02:33:46', b'1', '2020-12-17 02:34:55', NULL);
INSERT INTO `stock_trans` VALUES (8174, 0, 7, 1, 186, 5227, 16, 38, 8, 12, 12, 15, '2020-12-17 02:37:42', '2020-12-17 02:40:01', NULL, b'1', '2020-12-17 02:40:14', NULL);
INSERT INTO `stock_trans` VALUES (8175, 1, 7, 1, 184, 5192, 28, 18, 9, 13, 13, 17, '2020-12-17 02:41:29', '2020-12-17 02:41:30', '2020-12-17 02:44:48', b'1', '2020-12-17 02:46:06', NULL);
INSERT INTO `stock_trans` VALUES (8176, 1, 7, 1, 184, 5193, 21, 17, 9, 13, 13, 17, '2020-12-17 02:47:35', '2020-12-17 02:47:35', '2020-12-17 02:48:41', b'1', '2020-12-17 02:49:35', NULL);
INSERT INTO `stock_trans` VALUES (8177, 1, 7, 1, 184, 5194, 21, 18, 9, 13, 13, 17, '2020-12-17 02:55:04', '2020-12-17 02:55:04', '2020-12-17 02:58:09', b'1', '2020-12-17 02:59:13', NULL);
INSERT INTO `stock_trans` VALUES (8178, 1, 7, 1, 184, 5195, 21, 17, 9, 13, 13, 17, '2020-12-17 03:05:00', '2020-12-17 03:05:01', '2020-12-17 03:14:07', b'1', '2020-12-17 03:15:04', NULL);
INSERT INTO `stock_trans` VALUES (8179, 0, 7, 1, 186, 5229, 15, 35, 8, 12, 12, 15, '2020-12-17 03:06:57', '2020-12-17 03:08:21', '2020-12-17 03:11:09', b'1', '2020-12-17 03:11:10', NULL);
INSERT INTO `stock_trans` VALUES (8180, 0, 7, 1, 187, 5230, 14, 37, 7, 12, 12, 16, '2020-12-17 03:07:00', '2020-12-17 03:13:05', '2020-12-17 03:15:23', b'1', '2020-12-17 03:15:24', NULL);
INSERT INTO `stock_trans` VALUES (8181, 0, 7, 1, 186, 5231, 12, 35, 6, 12, 12, 15, '2020-12-17 03:11:11', '2020-12-17 03:16:49', '2020-12-17 03:19:15', b'1', '2020-12-17 03:19:16', NULL);
INSERT INTO `stock_trans` VALUES (8182, 0, 7, 1, 188, 5232, 9, 33, 5, 12, 12, 18, '2020-12-17 03:12:15', '2020-12-17 03:20:14', '2020-12-17 03:22:30', b'1', '2020-12-17 03:22:31', NULL);
INSERT INTO `stock_trans` VALUES (8183, 1, 7, 1, 184, 5196, 21, 18, 9, 13, 13, 17, '2020-12-17 03:15:06', '2020-12-17 03:15:49', '2020-12-17 03:17:52', b'1', '2020-12-17 03:18:54', NULL);
INSERT INTO `stock_trans` VALUES (8184, 0, 7, 1, 188, 5233, 10, 33, 5, 12, 12, 18, '2020-12-17 03:22:32', '2020-12-17 03:24:33', '2020-12-17 03:26:50', b'1', '2020-12-17 03:26:51', NULL);
INSERT INTO `stock_trans` VALUES (8185, 1, 7, 1, 184, 5197, 21, 17, 9, 13, 13, 17, '2020-12-17 03:25:38', '2020-12-17 03:25:39', '2020-12-17 03:26:48', b'1', '2020-12-17 03:27:59', NULL);
INSERT INTO `stock_trans` VALUES (8186, 0, 7, 1, 185, 5234, 4, 25, 2, 11, 11, 19, '2020-12-17 03:31:41', '2020-12-17 03:34:05', '2020-12-17 03:36:02', b'1', '2020-12-17 03:36:03', NULL);
INSERT INTO `stock_trans` VALUES (8187, 1, 7, 1, 184, 5198, 22, 18, 9, 13, 13, 17, '2020-12-17 03:35:29', '2020-12-17 03:35:30', '2020-12-17 03:43:46', b'1', '2020-12-17 03:44:47', NULL);
INSERT INTO `stock_trans` VALUES (8188, 0, 7, 1, 184, 5235, 2, 29, 1, 11, 11, 19, '2020-12-17 03:36:37', '2020-12-17 03:38:34', '2020-12-17 03:40:56', b'1', '2020-12-17 03:40:57', NULL);
INSERT INTO `stock_trans` VALUES (8189, 1, 7, 1, 184, 5199, 22, 17, 9, 13, 13, 17, '2020-12-17 03:46:26', '2020-12-17 03:46:26', '2020-12-17 03:47:31', b'1', '2020-12-17 03:48:27', NULL);
INSERT INTO `stock_trans` VALUES (8190, 0, 7, 1, 184, 5236, 5, 29, 3, 11, 11, 19, '2020-12-17 03:47:07', '2020-12-17 03:48:34', '2020-12-17 03:50:18', b'1', '2020-12-17 03:50:19', NULL);
INSERT INTO `stock_trans` VALUES (8191, 0, 7, 1, 185, 5237, 3, 25, 2, 11, 11, 18, '2020-12-17 03:49:45', '2020-12-17 03:52:21', '2020-12-17 03:54:18', b'1', '2020-12-17 03:54:19', NULL);
INSERT INTO `stock_trans` VALUES (8192, 0, 7, 1, 184, 5238, 1, 29, 1, 11, 11, 19, '2020-12-17 03:50:20', '2020-12-17 03:55:16', '2020-12-17 03:56:56', b'1', '2020-12-17 03:56:57', NULL);
INSERT INTO `stock_trans` VALUES (8193, 0, 7, 1, 192, 5239, 8, 21, 4, 11, 11, 18, '2020-12-17 03:56:15', '2020-12-17 03:59:22', '2020-12-17 04:02:32', b'1', '2020-12-17 04:02:33', NULL);
INSERT INTO `stock_trans` VALUES (8194, 0, 7, 1, 190, 5240, 16, 31, 8, 12, 12, 16, '2020-12-17 03:57:58', '2020-12-17 04:00:47', '2020-12-17 04:03:58', b'1', '2020-12-17 04:03:59', NULL);
INSERT INTO `stock_trans` VALUES (8195, 1, 7, 1, 184, 5200, 22, 18, 9, 13, 13, 17, '2020-12-17 03:58:11', '2020-12-17 03:58:11', '2020-12-17 04:06:42', b'1', '2020-12-17 04:07:44', NULL);
INSERT INTO `stock_trans` VALUES (8196, 1, 7, 1, 184, 5201, 22, 17, 9, 13, 13, 17, '2020-12-17 04:14:20', '2020-12-17 04:14:20', '2020-12-17 04:15:25', b'1', '2020-12-17 04:16:18', NULL);
INSERT INTO `stock_trans` VALUES (8197, 0, 7, 1, 190, 5241, 15, 31, 8, 12, 12, 16, '2020-12-17 04:17:12', '2020-12-17 04:19:19', '2020-12-17 04:22:03', b'1', '2020-12-17 04:22:04', NULL);
INSERT INTO `stock_trans` VALUES (8198, 0, 7, 1, 192, 5242, 2, 24, 1, 11, 11, 19, '2020-12-17 04:22:46', '2020-12-17 04:25:46', '2020-12-17 04:28:10', b'1', '2020-12-17 04:28:11', NULL);
INSERT INTO `stock_trans` VALUES (8199, 1, 7, 1, 188, 5218, 38, 49, 10, 13, 13, 18, '2020-12-17 04:22:47', '2020-12-17 04:25:44', '2020-12-17 04:27:07', b'1', '2020-12-17 04:28:21', NULL);
INSERT INTO `stock_trans` VALUES (8200, 1, 7, 1, 184, 5202, 22, 18, 9, 13, 13, 17, '2020-12-17 04:26:54', '2020-12-17 04:26:55', '2020-12-17 04:30:34', b'1', '2020-12-17 04:31:46', NULL);
INSERT INTO `stock_trans` VALUES (8201, 1, 7, 1, 188, 5222, 38, 50, 10, NULL, NULL, 18, '2020-12-17 04:29:09', '2020-12-17 04:29:10', NULL, b'1', '2020-12-17 04:29:36', NULL);
INSERT INTO `stock_trans` VALUES (8202, 0, 7, 1, 192, 5244, 7, 24, 4, 11, 11, 19, '2020-12-17 04:33:58', '2020-12-17 04:35:48', '2020-12-17 04:38:03', b'1', '2020-12-17 04:38:04', NULL);
INSERT INTO `stock_trans` VALUES (8203, 1, 7, 1, 184, 5204, 23, 17, 9, 13, 13, 17, '2020-12-17 04:36:52', '2020-12-17 04:36:52', '2020-12-17 04:38:36', b'1', '2020-12-17 04:39:29', NULL);
INSERT INTO `stock_trans` VALUES (8204, 0, 7, 1, 192, 5245, 6, 24, 3, 11, 11, 19, '2020-12-17 04:38:05', '2020-12-17 04:40:06', '2020-12-17 04:42:10', b'1', '2020-12-17 04:42:11', NULL);
INSERT INTO `stock_trans` VALUES (8205, 0, 7, 1, 191, 5243, 13, 36, 7, 12, 12, 15, '2020-12-17 04:39:08', '2020-12-17 04:42:26', '2020-12-17 04:45:00', b'1', '2020-12-17 04:45:01', NULL);
INSERT INTO `stock_trans` VALUES (8206, 0, 7, 1, 192, 5247, 1, 24, 1, 11, 11, 19, '2020-12-17 04:42:12', '2020-12-17 04:43:42', '2020-12-17 04:45:24', b'1', '2020-12-17 04:45:25', NULL);
INSERT INTO `stock_trans` VALUES (8207, 0, 7, 1, 190, 5248, 16, 31, 8, 12, 12, 16, '2020-12-17 04:45:25', '2020-12-17 04:55:56', '2020-12-17 04:58:40', b'1', '2020-12-17 04:58:41', NULL);
INSERT INTO `stock_trans` VALUES (8208, 1, 7, 1, 188, 5222, 38, 50, 10, 13, 13, 18, '2020-12-17 04:47:11', '2020-12-17 04:47:11', '2020-12-17 04:49:01', b'1', '2020-12-17 04:50:05', NULL);
INSERT INTO `stock_trans` VALUES (8209, 1, 7, 1, 184, 5205, 23, 18, 9, 13, 13, 17, '2020-12-17 04:47:21', '2020-12-17 04:47:21', '2020-12-17 04:57:04', b'1', '2020-12-17 04:58:01', NULL);
INSERT INTO `stock_trans` VALUES (8210, 0, 7, 1, 192, 5249, 2, 24, 1, 11, 11, 19, '2020-12-17 04:49:21', '2020-12-17 04:53:06', '2020-12-17 04:54:32', b'1', '2020-12-17 04:54:33', NULL);
INSERT INTO `stock_trans` VALUES (8211, 0, 7, 1, 190, 5250, 9, 31, 5, 12, 12, 16, '2020-12-17 04:58:43', '2020-12-17 05:00:14', '2020-12-17 05:01:54', b'1', '2020-12-17 05:01:55', NULL);
INSERT INTO `stock_trans` VALUES (8212, 1, 7, 1, 184, 5208, 23, 17, 9, 13, 13, 17, '2020-12-17 04:58:46', '2020-12-17 04:58:47', '2020-12-17 05:00:50', b'1', '2020-12-17 05:01:45', NULL);
INSERT INTO `stock_trans` VALUES (8213, 0, 7, 1, 192, 5251, 8, 22, 4, 11, 11, 19, '2020-12-17 05:05:39', '2020-12-17 05:08:09', '2020-12-17 05:10:58', b'1', '2020-12-17 05:10:59', NULL);
INSERT INTO `stock_trans` VALUES (8214, 0, 7, 1, 192, 5252, 5, 22, 3, 11, 11, 19, '2020-12-17 05:11:01', '2020-12-17 05:12:37', '2020-12-17 05:14:49', b'1', '2020-12-17 05:14:50', NULL);
INSERT INTO `stock_trans` VALUES (8215, 1, 7, 1, 184, 5213, 23, 18, 9, 13, 13, 17, '2020-12-17 05:11:14', '2020-12-17 05:11:14', '2020-12-17 05:13:04', b'1', '2020-12-17 05:14:04', NULL);
INSERT INTO `stock_trans` VALUES (8216, 0, 7, 1, 190, 5253, 11, 31, 6, 12, 12, 16, '2020-12-17 05:11:32', '2020-12-17 05:13:33', NULL, b'1', '2020-12-17 05:37:39', NULL);
INSERT INTO `stock_trans` VALUES (8217, 0, 7, 1, 192, 5254, 1, 22, 1, 11, 11, 19, '2020-12-17 05:14:50', '2020-12-17 05:16:13', '2020-12-17 05:18:03', b'1', '2020-12-17 05:18:04', NULL);
INSERT INTO `stock_trans` VALUES (8218, 0, 7, 1, 192, 5255, 2, 22, 1, 11, 11, 19, '2020-12-17 05:18:07', '2020-12-17 05:19:53', '2020-12-17 05:21:33', b'1', '2020-12-17 05:21:34', NULL);
INSERT INTO `stock_trans` VALUES (8219, 1, 7, 1, 184, 5214, 23, 17, 9, 13, 13, 17, '2020-12-17 05:22:40', '2020-12-17 05:22:41', '2020-12-17 05:24:25', b'1', '2020-12-17 05:25:48', NULL);
INSERT INTO `stock_trans` VALUES (8220, 1, 7, 1, 184, 5216, 27, 18, 9, 13, 13, 17, '2020-12-17 05:30:47', '2020-12-17 05:30:48', '2020-12-17 05:42:22', b'1', '2020-12-17 05:43:23', NULL);
INSERT INTO `stock_trans` VALUES (8221, 0, 7, 1, 192, 5256, 6, 22, 3, 11, 11, 19, '2020-12-17 05:35:46', '2020-12-17 05:37:55', '2020-12-17 05:39:36', b'1', '2020-12-17 05:39:37', NULL);
INSERT INTO `stock_trans` VALUES (8222, 0, 7, 1, 193, 5257, 4, 23, 2, 11, NULL, 15, '2020-12-17 05:35:49', NULL, NULL, b'1', '2020-12-17 05:43:53', NULL);
INSERT INTO `stock_trans` VALUES (8223, 1, 7, 1, 184, 5217, 27, 17, 9, 13, 13, 17, '2020-12-17 05:43:25', '2020-12-17 05:43:56', '2020-12-17 05:45:32', b'1', '2020-12-17 05:46:37', NULL);
INSERT INTO `stock_trans` VALUES (8224, 0, 7, 1, 192, 5258, 7, 23, 4, 11, NULL, 19, '2020-12-17 05:43:59', NULL, NULL, b'1', '2020-12-17 05:45:30', NULL);
INSERT INTO `stock_trans` VALUES (8225, 0, 7, 1, 193, 5257, 4, 28, 2, NULL, NULL, 15, '2020-12-17 05:44:00', NULL, NULL, b'1', '2020-12-17 05:45:33', NULL);
INSERT INTO `stock_trans` VALUES (8226, 0, 7, 1, 193, 5257, 4, 23, 2, 11, 11, 19, '2020-12-17 05:45:37', '2020-12-17 05:48:34', '2020-12-17 05:50:54', b'1', '2020-12-17 05:50:55', NULL);
INSERT INTO `stock_trans` VALUES (8227, 0, 7, 1, 192, 5260, 1, 28, 1, 11, NULL, 16, '2020-12-17 05:45:38', NULL, NULL, b'1', '2020-12-17 05:57:22', NULL);
INSERT INTO `stock_trans` VALUES (8228, 1, 7, 1, 184, 5220, 27, 18, 9, 13, 13, 17, '2020-12-17 05:50:13', '2020-12-17 05:50:14', '2020-12-17 06:03:14', b'1', '2020-12-17 06:04:37', NULL);
INSERT INTO `stock_trans` VALUES (8229, 0, 7, 1, 190, 5259, 15, 32, 8, NULL, NULL, 0, '2020-12-17 05:50:47', NULL, NULL, b'1', '2020-12-17 05:51:52', NULL);
INSERT INTO `stock_trans` VALUES (8230, 0, 7, 1, 190, 5259, 15, 32, 8, NULL, NULL, 0, '2020-12-17 05:52:07', NULL, NULL, b'1', '2020-12-17 05:53:18', NULL);
INSERT INTO `stock_trans` VALUES (8231, 0, 7, 1, 190, 5259, 15, 32, 8, NULL, NULL, 0, '2020-12-17 05:54:35', NULL, NULL, b'1', '2020-12-17 05:56:37', NULL);
INSERT INTO `stock_trans` VALUES (8232, 0, 7, 1, 192, 5258, 7, 28, 4, 11, 11, 19, '2020-12-17 05:57:39', '2020-12-17 06:02:56', '2020-12-17 06:05:11', b'1', '2020-12-17 06:05:12', NULL);
INSERT INTO `stock_trans` VALUES (8233, 0, 7, 1, 190, 5259, 15, 32, 8, 12, 12, 16, '2020-12-17 05:57:41', '2020-12-17 06:01:54', '2020-12-17 06:04:30', b'1', '2020-12-17 06:04:31', NULL);
INSERT INTO `stock_trans` VALUES (8234, 1, 7, 1, 184, 5221, 29, 17, 9, 13, 13, 17, '2020-12-17 06:04:40', '2020-12-17 06:04:49', '2020-12-17 06:06:15', b'1', '2020-12-17 06:07:28', NULL);
INSERT INTO `stock_trans` VALUES (8235, 0, 7, 1, 192, 5260, 1, 28, 1, 11, 11, 19, '2020-12-17 06:05:12', '2020-12-17 06:06:55', '2020-12-17 06:09:19', b'1', '2020-12-17 06:09:20', NULL);
INSERT INTO `stock_trans` VALUES (8236, 0, 7, 1, 190, 5262, 16, 32, 8, 12, 12, 16, '2020-12-17 06:07:06', '2020-12-17 06:09:49', '2020-12-17 06:12:29', b'1', '2020-12-17 06:12:30', NULL);
INSERT INTO `stock_trans` VALUES (8237, 0, 7, 1, 192, 5263, 2, 28, 1, 11, 11, 19, '2020-12-17 06:09:22', '2020-12-17 06:11:31', '2020-12-17 06:13:43', b'1', '2020-12-17 06:13:44', NULL);
INSERT INTO `stock_trans` VALUES (8238, 1, 7, 1, 184, 5224, 29, 18, 9, 13, 13, 17, '2020-12-17 06:12:37', '2020-12-17 06:12:38', '2020-12-17 06:16:23', b'1', '2020-12-17 06:17:30', NULL);
INSERT INTO `stock_trans` VALUES (8239, 0, 7, 1, 191, 5264, 14, 36, 7, 12, 12, 15, '2020-12-17 06:18:11', '2020-12-17 06:20:54', '2020-12-17 06:23:20', b'1', '2020-12-17 06:23:21', NULL);
INSERT INTO `stock_trans` VALUES (8240, 1, 7, 1, 184, 5235, 29, 17, 9, 13, 13, 17, '2020-12-17 06:22:05', '2020-12-17 06:22:06', '2020-12-17 06:27:02', b'1', '2020-12-17 06:28:18', NULL);
INSERT INTO `stock_trans` VALUES (8241, 0, 7, 1, 192, 5265, 8, 28, 4, 11, 11, 19, '2020-12-17 06:28:42', '2020-12-17 06:30:41', '2020-12-17 06:32:36', b'1', '2020-12-17 06:32:37', NULL);
INSERT INTO `stock_trans` VALUES (8242, 0, 7, 1, 192, 5266, 1, 28, 1, 11, 11, 19, '2020-12-17 06:32:37', '2020-12-17 06:34:19', '2020-12-17 06:35:58', b'1', '2020-12-17 06:35:59', NULL);
INSERT INTO `stock_trans` VALUES (8243, 1, 7, 1, 184, 5236, 29, 18, 9, 13, 13, 17, '2020-12-17 06:33:23', '2020-12-17 06:33:24', '2020-12-17 06:35:02', b'1', '2020-12-17 06:36:11', NULL);
INSERT INTO `stock_trans` VALUES (8244, 0, 7, 1, 194, 5267, 15, 32, 8, 12, 12, 16, '2020-12-17 06:35:22', '2020-12-17 06:37:48', '2020-12-17 06:39:40', b'1', '2020-12-17 06:39:41', NULL);
INSERT INTO `stock_trans` VALUES (8245, 0, 7, 1, 192, 5268, 5, 27, 3, 11, 11, 19, '2020-12-17 06:35:59', '2020-12-17 06:37:22', '2020-12-17 06:39:26', b'1', '2020-12-17 06:39:27', NULL);
INSERT INTO `stock_trans` VALUES (8246, 0, 7, 1, 192, 5270, 2, 27, 1, 11, 11, 19, '2020-12-17 06:40:59', '2020-12-17 06:43:05', '2020-12-17 06:45:29', b'1', '2020-12-17 06:45:30', NULL);
INSERT INTO `stock_trans` VALUES (8247, 0, 7, 1, 194, 5269, 10, 39, 5, 12, 12, 15, '2020-12-17 06:42:16', '2020-12-17 06:46:33', '2020-12-17 06:49:18', b'1', '2020-12-17 06:49:19', NULL);
INSERT INTO `stock_trans` VALUES (8248, 1, 7, 1, 179, 5127, 20, 17, 9, 13, 13, 19, '2020-12-17 06:45:36', '2020-12-17 06:47:54', '2020-12-17 06:49:00', b'1', '2020-12-17 06:49:57', NULL);
INSERT INTO `stock_trans` VALUES (8249, 1, 7, 1, 179, 5162, 20, 18, 9, 13, 13, 19, '2020-12-17 06:50:01', '2020-12-17 06:50:26', '2020-12-17 06:52:22', b'1', '2020-12-17 06:53:15', NULL);
INSERT INTO `stock_trans` VALUES (8250, 0, 7, 1, 192, 5272, 7, 27, 4, 11, 11, 17, '2020-12-17 06:51:45', '2020-12-17 06:53:46', '2020-12-17 06:55:50', b'1', '2020-12-17 06:55:51', NULL);
INSERT INTO `stock_trans` VALUES (8251, 0, 7, 1, 195, 5273, 13, 36, 7, 12, 12, 15, '2020-12-17 06:53:57', '2020-12-17 06:55:20', '2020-12-17 06:57:16', b'1', '2020-12-17 06:57:17', NULL);
INSERT INTO `stock_trans` VALUES (8252, 0, 7, 1, 192, 5274, 1, 27, 1, 11, 11, 17, '2020-12-17 06:55:51', '2020-12-17 06:57:32', '2020-12-17 06:59:32', b'1', '2020-12-17 06:59:33', NULL);
INSERT INTO `stock_trans` VALUES (8253, 0, 7, 1, 192, 5275, 6, 27, 3, 11, 11, 17, '2020-12-17 06:59:34', '2020-12-17 07:01:35', '2020-12-17 07:03:04', b'1', '2020-12-17 07:03:05', NULL);
INSERT INTO `stock_trans` VALUES (8254, 1, 7, 1, 179, 5210, 26, 17, 9, 13, 13, 19, '2020-12-17 07:15:16', '2020-12-17 07:15:17', '2020-12-17 07:16:55', b'1', '2020-12-17 07:18:08', NULL);
INSERT INTO `stock_trans` VALUES (8255, 0, 7, 1, 192, 5276, 2, 20, 1, 11, 11, 17, '2020-12-17 07:16:33', '2020-12-17 07:18:53', '2020-12-17 07:21:24', b'1', '2020-12-17 07:21:25', NULL);
INSERT INTO `stock_trans` VALUES (8256, 0, 7, 1, 194, 5277, 16, 39, 8, 12, 12, 15, '2020-12-17 07:16:33', '2020-12-17 07:19:01', '2020-12-17 07:21:26', b'1', '2020-12-17 07:21:27', NULL);
INSERT INTO `stock_trans` VALUES (8257, 0, 7, 1, 194, 5278, 12, 39, 6, 12, 12, 15, '2020-12-17 07:21:28', '2020-12-17 07:23:33', '2020-12-17 07:25:33', b'1', '2020-12-17 07:25:34', NULL);
INSERT INTO `stock_trans` VALUES (8258, 1, 7, 1, 185, 5190, 19, 18, 9, 13, 13, 17, '2020-12-17 07:23:03', '2020-12-17 07:25:53', '2020-12-17 07:27:13', b'1', '2020-12-17 07:28:22', NULL);
INSERT INTO `stock_trans` VALUES (8259, 0, 7, 1, 192, 5279, 8, 20, 4, 11, 11, 19, '2020-12-17 07:26:58', '2020-12-17 07:29:44', '2020-12-17 07:32:29', b'1', '2020-12-17 07:32:30', NULL);
INSERT INTO `stock_trans` VALUES (8260, 0, 7, 1, 195, 5280, 14, 36, 7, 12, 12, 15, '2020-12-17 07:26:59', '2020-12-17 07:29:04', '2020-12-17 07:30:41', b'1', '2020-12-17 07:30:43', NULL);
INSERT INTO `stock_trans` VALUES (8261, 0, 7, 1, 192, 5281, 1, 20, 1, 11, 11, 19, '2020-12-17 07:32:30', '2020-12-17 07:33:50', '2020-12-17 07:35:35', b'1', '2020-12-17 07:35:36', NULL);
INSERT INTO `stock_trans` VALUES (8262, 1, 7, 1, 185, 5206, 25, 17, 9, 13, 13, 17, '2020-12-17 07:34:03', '2020-12-17 07:34:03', '2020-12-17 07:35:45', b'1', '2020-12-17 07:36:45', NULL);
INSERT INTO `stock_trans` VALUES (8263, 0, 7, 1, 192, 5282, 5, 20, 3, 11, 11, 19, '2020-12-17 07:35:38', '2020-12-17 07:37:18', '2020-12-17 07:39:13', b'1', '2020-12-17 07:39:14', NULL);
INSERT INTO `stock_trans` VALUES (8264, 0, 7, 1, 192, 5283, 2, 20, 1, 11, 11, 19, '2020-12-17 07:41:39', '2020-12-17 07:43:30', '2020-12-17 07:44:55', b'1', '2020-12-17 07:44:57', NULL);
INSERT INTO `stock_trans` VALUES (8265, 0, 7, 1, 194, 5284, 15, 39, 8, 12, 12, 15, '2020-12-17 07:42:43', '2020-12-17 07:44:37', '2020-12-17 07:46:02', b'1', '2020-12-17 07:46:03', NULL);
INSERT INTO `stock_trans` VALUES (8266, 1, 7, 1, 185, 5234, 25, 18, 9, 13, 13, 17, '2020-12-17 07:44:03', '2020-12-17 07:44:03', '2020-12-17 07:52:36', b'1', '2020-12-17 07:53:27', NULL);
INSERT INTO `stock_trans` VALUES (8267, 1, 7, 1, 185, 5237, 25, 17, 9, 13, 13, 17, '2020-12-17 07:53:41', '2020-12-17 07:54:06', '2020-12-17 07:55:44', b'1', '2020-12-17 07:57:00', NULL);
INSERT INTO `stock_trans` VALUES (8268, 0, 7, 1, 192, 5285, 7, 19, 4, 11, 11, 19, '2020-12-17 07:54:47', '2020-12-17 07:56:41', '2020-12-17 07:59:29', b'1', '2020-12-17 07:59:30', NULL);
INSERT INTO `stock_trans` VALUES (8269, 0, 7, 1, 192, 5286, 6, 19, 3, 11, 11, 19, '2020-12-17 07:59:31', '2020-12-17 08:01:49', '2020-12-17 08:04:24', b'1', '2020-12-17 08:04:25', NULL);
INSERT INTO `stock_trans` VALUES (8270, 1, 7, 1, 195, 5243, 36, 18, 9, 13, NULL, 17, '2020-12-17 08:02:40', '2020-12-17 08:18:01', NULL, b'1', '2020-12-17 09:47:16', b'1');
INSERT INTO `stock_trans` VALUES (8271, 0, 7, 1, 192, 5289, 1, 19, 1, 11, 11, 19, '2020-12-17 08:04:27', '2020-12-17 08:05:49', '2020-12-17 08:07:37', b'1', '2020-12-17 08:07:38', NULL);
INSERT INTO `stock_trans` VALUES (8272, 0, 7, 1, 192, 5291, 2, 19, 1, 11, 11, 19, '2020-12-17 08:07:39', '2020-12-17 08:09:33', '2020-12-17 08:11:21', b'1', '2020-12-17 08:11:22', NULL);
INSERT INTO `stock_trans` VALUES (8273, 0, 7, 1, 192, 5295, 8, 19, 4, 11, 11, 19, '2020-12-17 08:20:48', '2020-12-17 08:23:18', '2020-12-17 08:25:18', b'1', '2020-12-17 08:25:19', NULL);
INSERT INTO `stock_trans` VALUES (8274, 0, 7, 1, 192, 5296, 1, 25, 1, 11, 11, 19, '2020-12-17 08:25:20', '2020-12-17 08:26:43', '2020-12-17 08:29:11', b'1', '2020-12-17 08:29:12', NULL);
INSERT INTO `stock_trans` VALUES (8275, 0, 7, 1, 192, 5297, 5, 25, 3, 11, 11, 19, '2020-12-17 08:29:13', '2020-12-17 08:30:36', '2020-12-17 08:32:34', b'1', '2020-12-17 08:32:35', NULL);
INSERT INTO `stock_trans` VALUES (8276, 0, 7, 1, 192, 5298, 2, 25, 1, 11, 11, 19, '2020-12-17 08:33:57', '2020-12-17 08:35:57', '2020-12-17 08:38:03', b'1', '2020-12-17 08:38:04', NULL);
INSERT INTO `stock_trans` VALUES (8277, 0, 7, 1, 192, 5300, 7, 25, 4, 11, 11, 19, '2020-12-17 08:51:14', '2020-12-17 08:52:49', '2020-12-17 08:54:38', b'1', '2020-12-17 08:54:39', NULL);
INSERT INTO `stock_trans` VALUES (8278, 0, 7, 1, 192, 5301, 6, 25, 3, 11, 11, 19, '2020-12-17 08:54:39', '2020-12-17 08:56:36', '2020-12-17 08:58:02', b'1', '2020-12-17 08:58:03', NULL);
INSERT INTO `stock_trans` VALUES (8279, 0, 7, 1, 192, 5302, 1, 26, 1, 11, 11, 19, '2020-12-17 08:58:04', '2020-12-17 08:59:37', '2020-12-17 09:01:58', b'1', '2020-12-17 09:01:59', NULL);
INSERT INTO `stock_trans` VALUES (8280, 0, 7, 1, 192, 5303, 2, 26, 1, 11, 11, 19, '2020-12-17 09:12:23', '2020-12-17 09:14:25', '2020-12-17 09:16:42', b'1', '2020-12-17 09:16:43', NULL);
INSERT INTO `stock_trans` VALUES (8281, 0, 7, 1, 193, 5304, 3, 23, 2, 11, 11, 19, '2020-12-17 09:20:36', '2020-12-17 09:21:58', '2020-12-17 09:24:00', b'1', '2020-12-17 09:24:01', NULL);
INSERT INTO `stock_trans` VALUES (8282, 0, 7, 1, 196, 5294, 16, 44, 8, 12, 12, 18, '2020-12-17 09:22:49', '2020-12-17 09:25:52', '2020-12-17 09:28:10', b'1', '2020-12-17 09:28:11', NULL);
INSERT INTO `stock_trans` VALUES (8283, 0, 7, 1, 192, 5307, 1, 26, 1, 11, 11, 19, '2020-12-17 09:43:37', '2020-12-17 09:45:03', '2020-12-17 09:47:02', b'1', '2020-12-17 09:47:03', NULL);
INSERT INTO `stock_trans` VALUES (8284, 0, 7, 1, 192, 5308, 5, 26, 3, 11, 11, 19, '2020-12-17 09:47:04', '2020-12-17 09:48:22', '2020-12-17 09:49:53', b'1', '2020-12-17 09:49:54', NULL);
INSERT INTO `stock_trans` VALUES (8285, 0, 7, 1, 192, 5309, 8, 26, 4, 11, 11, 19, '2020-12-17 09:49:54', '2020-12-17 09:51:59', '2020-12-17 09:53:36', b'1', '2020-12-17 09:53:37', NULL);
INSERT INTO `stock_trans` VALUES (8286, 1, 7, 1, 192, 5242, 24, 17, 9, 13, 13, 16, '2020-12-17 09:52:28', '2020-12-17 09:54:49', '2020-12-17 09:55:59', b'1', '2020-12-17 09:56:57', NULL);
INSERT INTO `stock_trans` VALUES (8287, 0, 7, 1, 192, 5311, 2, 29, 1, 11, 11, 19, '2020-12-17 09:54:55', '2020-12-17 09:56:59', '2020-12-17 09:59:39', b'1', '2020-12-17 09:59:40', NULL);
INSERT INTO `stock_trans` VALUES (8288, 1, 7, 1, 192, 5244, 24, 18, 9, 13, 13, 16, '2020-12-17 09:55:00', '2020-12-17 09:55:01', '2020-12-17 09:57:26', b'1', '2020-12-17 09:58:21', NULL);
INSERT INTO `stock_trans` VALUES (8289, 1, 7, 1, 192, 5245, 24, 17, 9, 13, 13, 16, '2020-12-17 09:58:22', '2020-12-17 09:59:27', '2020-12-17 10:01:04', b'1', '2020-12-17 10:02:11', NULL);
INSERT INTO `stock_trans` VALUES (8290, 0, 7, 1, 192, 5317, 7, 29, 4, 11, 11, 19, '2020-12-17 10:02:47', '2020-12-17 10:04:16', '2020-12-17 10:06:11', b'1', '2020-12-17 10:06:12', NULL);
INSERT INTO `stock_trans` VALUES (8291, 1, 7, 1, 192, 5245, 24, 18, 9, 13, 13, 16, '2020-12-17 10:05:53', '2020-12-17 10:05:53', '2020-12-17 10:09:17', b'1', '2020-12-17 10:10:12', NULL);
INSERT INTO `stock_trans` VALUES (8292, 0, 7, 1, 192, 5318, 6, 29, 3, 11, 11, 19, '2020-12-17 10:06:13', '2020-12-17 10:09:48', '2020-12-17 10:11:45', b'1', '2020-12-17 10:11:46', NULL);
INSERT INTO `stock_trans` VALUES (8293, 0, 7, 1, 192, 5320, 1, 29, 1, 11, NULL, 19, '2020-12-17 10:11:46', NULL, NULL, b'1', '2020-12-17 15:25:13', b'1');

-- ----------------------------
-- Table structure for tile_track
-- ----------------------------
DROP TABLE IF EXISTS `tile_track`;
CREATE TABLE `tile_track`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '砖机轨道',
  `tile_id` int(11) UNSIGNED NOT NULL COMMENT '砖机ID',
  `track_id` int(11) UNSIGNED NOT NULL,
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '优先级',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for track
-- ----------------------------
DROP TABLE IF EXISTS `track`;
CREATE TABLE `track`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '标识',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `area` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '区域：过滤作用',
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
  `order` smallint(6) NULL DEFAULT NULL COMMENT '顺序',
  `recent_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '最近上砖/下砖规格',
  `recent_tileid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '最近上/下砖机ID',
  `alert_status` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '故障状态',
  `alert_carrier` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '故障小车',
  `alert_trans` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '故障任务',
  `early_full` bit(1) NULL DEFAULT NULL COMMENT '提前满砖',
  `full_time` datetime(0) NULL DEFAULT NULL COMMENT '满砖时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 51 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '轨道' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of track
-- ----------------------------
INSERT INTO `track` VALUES (1, '01_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 101, 100, 0, 0, 0, NULL, 101, NULL, NULL, NULL, NULL, NULL, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (2, '02_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 102, 100, 0, 0, 0, NULL, 102, NULL, NULL, NULL, NULL, NULL, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (3, '03_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 103, 100, 0, 0, 0, NULL, 103, NULL, NULL, NULL, NULL, NULL, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (4, '04_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 104, 100, 0, 0, 0, NULL, 104, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (5, '05_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 105, 100, 0, 0, 0, NULL, 105, NULL, NULL, NULL, NULL, NULL, 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (6, '06_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 106, 100, 0, 0, 0, NULL, 106, NULL, NULL, NULL, NULL, NULL, 10, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (7, '07_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 107, 100, 0, 0, 0, NULL, 107, NULL, NULL, NULL, NULL, NULL, 12, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (8, '08_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 108, 100, 0, 0, 0, NULL, 108, NULL, NULL, NULL, NULL, NULL, 13, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (9, '09_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 109, 100, 0, 0, 0, NULL, 109, NULL, NULL, NULL, NULL, NULL, 15, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (10, '10_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 110, 100, 0, 0, 0, NULL, 110, NULL, NULL, NULL, NULL, NULL, 16, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (11, '11_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 111, 100, 0, 0, 0, NULL, 111, NULL, NULL, NULL, NULL, NULL, 18, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (12, '12_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 112, 100, 0, 0, 0, NULL, 112, NULL, NULL, NULL, NULL, NULL, 19, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (13, '13_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 113, 100, 0, 0, 0, NULL, 113, NULL, NULL, NULL, NULL, NULL, 21, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (14, '14_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 114, 100, 0, 0, 0, NULL, 114, NULL, NULL, NULL, NULL, NULL, 22, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (15, '15_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 115, 100, 0, 0, 0, NULL, 115, NULL, NULL, NULL, NULL, NULL, 24, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (16, '16_下砖轨', 1, 1, 1, 1, 0, 0, 0, 0, 116, 100, 0, 0, 0, NULL, 116, NULL, NULL, NULL, NULL, NULL, 25, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (17, '01_上砖轨', 1, 0, 1, 1, 0, 0, 0, 601, 0, 100, 0, 0, 0, NULL, 601, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (18, '02_上砖轨', 1, 0, 1, 1, 0, 0, 0, 602, 0, 100, 0, 0, 0, NULL, 602, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (19, '01_储砖轨', 1, 4, 2, 1, 700, 350, 350, 201, 501, 100, 0, 0, 20, NULL, 201, 501, NULL, NULL, NULL, NULL, 1, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (20, '02_储砖轨', 1, 4, 2, 1, 700, 350, 350, 202, 502, 100, 0, 19, 21, NULL, 202, 502, NULL, NULL, NULL, NULL, 2, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (21, '03_储砖轨', 1, 4, 2, 1, 700, 350, 350, 203, 503, 100, 0, 20, 22, NULL, 203, 503, NULL, NULL, NULL, NULL, 3, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (22, '04_储砖轨', 1, 4, 2, 1, 700, 350, 350, 204, 504, 100, 0, 21, 23, NULL, 204, 504, NULL, NULL, NULL, NULL, 4, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (23, '05_储砖轨', 1, 4, 1, 1, 700, 350, 350, 205, 505, 100, 0, 22, 24, NULL, 205, 505, NULL, NULL, NULL, NULL, 5, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (24, '06_储砖轨', 1, 4, 2, 1, 700, 350, 350, 206, 506, 100, 0, 23, 25, NULL, 206, 506, NULL, NULL, NULL, NULL, 6, 192, 9, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (25, '07_储砖轨', 1, 4, 2, 1, 700, 350, 350, 207, 507, 100, 0, 24, 26, NULL, 207, 507, NULL, NULL, NULL, NULL, 7, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (26, '08_储砖轨', 1, 4, 2, 1, 700, 350, 350, 208, 508, 100, 0, 25, 27, NULL, 208, 508, NULL, NULL, NULL, NULL, 8, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (27, '09_储砖轨', 1, 4, 2, 1, 700, 350, 350, 209, 509, 100, 0, 26, 28, NULL, 209, 509, NULL, NULL, NULL, NULL, 9, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (28, '10_储砖轨', 1, 4, 2, 1, 700, 350, 350, 210, 510, 100, 0, 27, 29, NULL, 210, 510, NULL, NULL, NULL, NULL, 10, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (29, '11_储砖轨', 1, 4, 1, 1, 700, 350, 350, 211, 511, 100, 0, 28, 30, NULL, 211, 511, NULL, NULL, NULL, NULL, 11, 192, 1, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (30, '12_储砖轨', 1, 4, 0, 1, 700, 350, 350, 212, 512, 100, 0, 29, 31, NULL, 212, 512, NULL, NULL, NULL, NULL, 12, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (31, '13_储砖轨', 1, 4, 2, 1, 700, 350, 350, 213, 513, 100, 0, 30, 32, NULL, 213, 513, NULL, NULL, NULL, NULL, 13, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (32, '14_储砖轨', 1, 4, 2, 1, 700, 350, 350, 214, 514, 100, 0, 31, 33, NULL, 214, 514, NULL, NULL, NULL, NULL, 14, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (33, '15_储砖轨', 1, 4, 2, 1, 700, 350, 350, 215, 515, 100, 0, 32, 34, NULL, 215, 515, NULL, NULL, NULL, NULL, 15, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (34, '16_储砖轨', 1, 4, 0, 1, 700, 350, 350, 216, 516, 100, 0, 33, 35, NULL, 216, 516, NULL, NULL, NULL, NULL, 16, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (35, '17_储砖轨', 1, 4, 2, 1, 700, 350, 350, 217, 517, 100, 0, 34, 36, NULL, 217, 517, NULL, NULL, NULL, NULL, 17, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (36, '18_储砖轨', 1, 4, 2, 1, 700, 350, 350, 218, 518, 100, 0, 35, 37, NULL, 218, 518, NULL, NULL, NULL, NULL, 18, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (37, '19_储砖轨', 1, 4, 2, 1, 700, 350, 350, 219, 519, 100, 0, 36, 38, NULL, 219, 519, NULL, NULL, NULL, NULL, 19, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (38, '20_储砖轨', 1, 4, 2, 1, 700, 350, 350, 220, 520, 100, 0, 37, 39, NULL, 220, 520, NULL, NULL, NULL, NULL, 20, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (39, '21_储砖轨', 1, 4, 2, 1, 700, 350, 350, 221, 521, 100, 0, 38, 40, NULL, 221, 521, NULL, NULL, NULL, NULL, 21, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (40, '22_储砖轨', 1, 4, 2, 1, 700, 350, 350, 222, 522, 100, 0, 39, 41, NULL, 222, 522, NULL, NULL, NULL, NULL, 22, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (41, '23_储砖轨', 1, 4, 0, 1, 700, 350, 350, 223, 523, 100, 0, 40, 42, NULL, 223, 523, NULL, NULL, NULL, NULL, 23, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (42, '24_储砖轨', 1, 4, 2, 1, 700, 350, 350, 224, 524, 100, 0, 41, 43, NULL, 224, 524, NULL, NULL, NULL, NULL, 24, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (43, '25_储砖轨', 1, 4, 0, 1, 700, 350, 350, 225, 525, 100, 0, 42, 44, NULL, 225, 525, NULL, NULL, NULL, NULL, 25, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (44, '26_储砖轨', 1, 4, 1, 1, 700, 350, 350, 226, 526, 100, 0, 43, 0, NULL, 226, 526, NULL, NULL, NULL, NULL, 26, 0, 0, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (45, 'B1_摆渡轨', 1, 5, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 701, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (46, 'B2_摆渡轨', 1, 5, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 702, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (47, 'B5_摆渡轨', 1, 6, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 741, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (48, 'B6_摆渡轨', 1, 6, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 742, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (49, '03_上砖轨', 1, 0, 1, 1, 0, 0, 0, 603, 0, 100, 0, 0, 0, NULL, 603, NULL, NULL, NULL, NULL, NULL, 14, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (50, '04_上砖轨', 1, 0, 1, 1, 0, 0, 0, 604, 0, 100, 0, 0, 0, NULL, 604, NULL, NULL, NULL, NULL, NULL, 17, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 3095 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '轨道空满记录' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of track_log
-- ----------------------------
INSERT INTO `track_log` VALUES (2891, 19, 2, 19, 5, '2020-12-16 00:28:04', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2892, 38, 1, 17, 0, '2020-12-16 00:40:57', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2893, 22, 1, 15, 1, '2020-12-16 00:48:31', '有货', 1);
INSERT INTO `track_log` VALUES (2894, 44, 2, 16, 5, '2020-12-16 00:49:05', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2895, 22, 1, 15, 0, '2020-12-16 00:58:20', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2896, 20, 2, 17, 5, '2020-12-16 01:12:11', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2897, 43, 2, 16, 5, '2020-12-16 01:17:52', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2898, 27, 1, 15, 0, '2020-12-16 01:23:35', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2899, 33, 1, 19, 1, '2020-12-16 01:24:24', '有货', 1);
INSERT INTO `track_log` VALUES (2900, 33, 1, 19, 0, '2020-12-16 01:32:07', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2901, 21, 2, 17, 5, '2020-12-16 01:43:05', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2902, 42, 2, 16, 5, '2020-12-16 01:47:01', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2903, 44, 1, 19, 1, '2020-12-16 02:14:02', '有货', 1);
INSERT INTO `track_log` VALUES (2904, 19, 1, 15, 1, '2020-12-16 02:17:50', '有货', 1);
INSERT INTO `track_log` VALUES (2905, 22, 2, 17, 5, '2020-12-16 02:18:14', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2906, 44, 1, 19, 0, '2020-12-16 02:20:51', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2907, 19, 1, 15, 0, '2020-12-16 02:25:21', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2908, 41, 2, 18, 5, '2020-12-16 02:31:23', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2909, 24, 2, 17, 5, '2020-12-16 02:46:35', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2910, 44, 2, 18, 5, '2020-12-16 03:02:44', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2911, 43, 1, 19, 1, '2020-12-16 03:05:12', '有货', 1);
INSERT INTO `track_log` VALUES (2912, 20, 1, 15, 1, '2020-12-16 03:09:40', '有货', 1);
INSERT INTO `track_log` VALUES (2913, 43, 1, 19, 0, '2020-12-16 03:12:56', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2914, 20, 1, 15, 0, '2020-12-16 03:17:02', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2915, 19, 2, 17, 5, '2020-12-16 03:23:12', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2916, 39, 2, 18, 5, '2020-12-16 03:34:37', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2917, 20, 2, 17, 5, '2020-12-16 03:51:36', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2918, 43, 2, 16, 5, '2020-12-16 03:56:48', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2919, 21, 1, 15, 1, '2020-12-16 03:59:34', '有货', 1);
INSERT INTO `track_log` VALUES (2920, 42, 1, 19, 1, '2020-12-16 04:03:12', '有货', 1);
INSERT INTO `track_log` VALUES (2921, 21, 1, 15, 0, '2020-12-16 04:05:45', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2922, 42, 1, 19, 0, '2020-12-16 04:09:24', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2923, 25, 2, 17, 5, '2020-12-16 04:18:07', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2924, 38, 2, 16, 5, '2020-12-16 04:37:12', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2925, 21, 2, 18, 5, '2020-12-16 04:47:04', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2926, 22, 1, 15, 1, '2020-12-16 04:47:44', '有货', 1);
INSERT INTO `track_log` VALUES (2927, 22, 1, 15, 0, '2020-12-16 04:55:10', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2928, 41, 1, 19, 1, '2020-12-16 05:04:11', '有货', 1);
INSERT INTO `track_log` VALUES (2929, 41, 1, 19, 0, '2020-12-16 05:10:03', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2930, 42, 2, 16, 5, '2020-12-16 05:30:07', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2931, 24, 1, 15, 1, '2020-12-16 05:40:20', '有货', 1);
INSERT INTO `track_log` VALUES (2932, 22, 2, 18, 5, '2020-12-16 05:48:20', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2933, 24, 1, 15, 0, '2020-12-16 05:48:43', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2934, 44, 1, 19, 1, '2020-12-16 05:56:41', '有货', 1);
INSERT INTO `track_log` VALUES (2935, 44, 1, 19, 0, '2020-12-16 06:02:02', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2936, 41, 2, 16, 5, '2020-12-16 06:06:08', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2937, 26, 2, 18, 5, '2020-12-16 06:17:09', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2938, 19, 1, 15, 1, '2020-12-16 06:37:15', '有货', 1);
INSERT INTO `track_log` VALUES (2939, 19, 1, 15, 0, '2020-12-16 06:41:54', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2940, 24, 2, 17, 5, '2020-12-16 06:43:08', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2941, 44, 2, 16, 5, '2020-12-16 06:47:18', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2942, 39, 1, 19, 1, '2020-12-16 06:49:32', '有货', 1);
INSERT INTO `track_log` VALUES (2943, 39, 1, 19, 0, '2020-12-16 07:04:01', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2944, 27, 2, 17, 5, '2020-12-16 07:10:40', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2945, 20, 1, 15, 1, '2020-12-16 07:30:01', '有货', 1);
INSERT INTO `track_log` VALUES (2946, 43, 1, 19, 1, '2020-12-16 07:39:08', '有货', 1);
INSERT INTO `track_log` VALUES (2947, 20, 1, 15, 0, '2020-12-16 07:39:10', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2948, 43, 1, 19, 0, '2020-12-16 07:44:15', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2949, 19, 2, 18, 5, '2020-12-16 07:47:26', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2950, 37, 2, 16, 5, '2020-12-16 08:07:33', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2951, 25, 1, 15, 1, '2020-12-16 08:16:03', '有货', 1);
INSERT INTO `track_log` VALUES (2952, 25, 1, 15, 0, '2020-12-16 08:20:53', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2953, 38, 1, 19, 1, '2020-12-16 08:21:22', '有货', 1);
INSERT INTO `track_log` VALUES (2954, 38, 1, 19, 0, '2020-12-16 08:29:59', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2955, 20, 2, 17, 5, '2020-12-16 08:31:37', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2956, 43, 2, 16, 5, '2020-12-16 08:53:05', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2957, 21, 1, 15, 1, '2020-12-16 08:57:29', '有货', 1);
INSERT INTO `track_log` VALUES (2958, 21, 1, 15, 0, '2020-12-16 09:04:47', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2959, 25, 2, 17, 5, '2020-12-16 09:12:14', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2960, 42, 1, 19, 1, '2020-12-16 09:13:12', '有货', 1);
INSERT INTO `track_log` VALUES (2961, 42, 1, 19, 0, '2020-12-16 09:24:25', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2962, 22, 1, 15, 1, '2020-12-16 09:39:21', '有货', 1);
INSERT INTO `track_log` VALUES (2963, 21, 2, 17, 5, '2020-12-16 09:42:02', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2964, 22, 1, 15, 0, '2020-12-16 09:44:20', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2965, 38, 2, 18, 5, '2020-12-16 09:46:28', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2966, 41, 1, 19, 1, '2020-12-16 09:59:01', '有货', 1);
INSERT INTO `track_log` VALUES (2967, 41, 1, 19, 0, '2020-12-16 10:04:50', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2968, 42, 2, 16, 5, '2020-12-16 10:18:03', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2969, 26, 1, 15, 1, '2020-12-16 10:22:23', '有货', 1);
INSERT INTO `track_log` VALUES (2970, 26, 1, 15, 0, '2020-12-16 10:26:16', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2971, 41, 2, 16, 5, '2020-12-16 10:43:32', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2972, 44, 1, 19, 1, '2020-12-16 10:44:50', '有货', 1);
INSERT INTO `track_log` VALUES (2973, 44, 1, 19, 0, '2020-12-16 10:48:07', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2974, 24, 1, 15, 1, '2020-12-16 11:03:41', '有货', 1);
INSERT INTO `track_log` VALUES (2975, 24, 1, 15, 0, '2020-12-16 11:08:19', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2976, 26, 2, 17, 5, '2020-12-16 11:11:51', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2977, 36, 2, 16, 5, '2020-12-16 11:16:01', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2978, 37, 1, 19, 1, '2020-12-16 11:23:55', '有货', 1);
INSERT INTO `track_log` VALUES (2979, 37, 1, 19, 0, '2020-12-16 11:29:08', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2980, 44, 2, 16, 5, '2020-12-16 11:42:11', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2981, 27, 1, 15, 1, '2020-12-16 11:44:55', '有货', 1);
INSERT INTO `track_log` VALUES (2982, 27, 1, 15, 0, '2020-12-16 11:51:30', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2983, 24, 2, 17, 5, '2020-12-16 11:53:22', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2984, 43, 1, 19, 1, '2020-12-16 12:09:12', '有货', 1);
INSERT INTO `track_log` VALUES (2985, 37, 2, 16, 5, '2020-12-16 12:13:26', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2986, 43, 1, 19, 0, '2020-12-16 12:14:07', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2987, 39, 1, 19, 0, '2020-12-16 12:29:40', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2988, 19, 1, 15, 1, '2020-12-16 12:29:57', '有货', 1);
INSERT INTO `track_log` VALUES (2989, 30, 2, 17, 5, '2020-12-16 12:33:03', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2990, 19, 1, 15, 0, '2020-12-16 12:33:28', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2991, 35, 2, 16, 5, '2020-12-16 12:43:24', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2992, 40, 1, 18, 0, '2020-12-16 12:53:24', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2993, 31, 2, 17, 5, '2020-12-16 13:08:07', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2994, 43, 2, 19, 5, '2020-12-16 13:09:11', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2995, 20, 1, 15, 1, '2020-12-16 13:09:31', '有货', 1);
INSERT INTO `track_log` VALUES (2996, 20, 1, 15, 0, '2020-12-16 13:14:14', '无库存数据', 1);
INSERT INTO `track_log` VALUES (2997, 40, 2, 18, 5, '2020-12-16 13:39:48', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (2998, 38, 1, 17, 1, '2020-12-16 13:46:41', '有货', 1);
INSERT INTO `track_log` VALUES (2999, 19, 2, 16, 5, '2020-12-16 13:47:11', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3000, 38, 1, 17, 0, '2020-12-16 13:58:42', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3001, 25, 1, 15, 1, '2020-12-16 13:59:17', '有货', 1);
INSERT INTO `track_log` VALUES (3002, 25, 1, 15, 0, '2020-12-16 14:02:36', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3003, 32, 2, 16, 5, '2020-12-16 14:18:22', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3004, 34, 2, 18, 5, '2020-12-16 14:19:28', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3005, 42, 1, 17, 1, '2020-12-16 14:32:04', '有货', 1);
INSERT INTO `track_log` VALUES (3006, 42, 1, 17, 0, '2020-12-16 14:35:05', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3007, 21, 1, 15, 1, '2020-12-16 14:35:18', '有货', 1);
INSERT INTO `track_log` VALUES (3008, 21, 1, 15, 0, '2020-12-16 14:39:25', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3009, 25, 2, 19, 5, '2020-12-16 14:46:17', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3010, 41, 1, 17, 1, '2020-12-16 15:09:00', '有货', 1);
INSERT INTO `track_log` VALUES (3011, 41, 1, 17, 0, '2020-12-16 15:12:05', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3012, 21, 2, 19, 5, '2020-12-16 15:13:00', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3013, 22, 1, 15, 0, '2020-12-16 15:15:43', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3014, 28, 1, 15, 0, '2020-12-16 15:50:19', '无货', 1);
INSERT INTO `track_log` VALUES (3015, 33, 2, 15, 5, '2020-12-16 15:56:22', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3016, 36, 1, 17, 1, '2020-12-16 15:58:20', '有货', 1);
INSERT INTO `track_log` VALUES (3017, 36, 1, 17, 0, '2020-12-16 16:00:10', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3018, 26, 1, 19, 1, '2020-12-16 16:18:43', '有货', 1);
INSERT INTO `track_log` VALUES (3019, 26, 1, 19, 0, '2020-12-16 16:23:52', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3020, 44, 1, 17, 1, '2020-12-16 16:50:39', '有货', 1);
INSERT INTO `track_log` VALUES (3021, 44, 1, 17, 0, '2020-12-16 16:53:24', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3022, 26, 2, 15, 4, '2020-12-16 16:54:30', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3023, 24, 1, 19, 1, '2020-12-16 17:02:34', '有货', 1);
INSERT INTO `track_log` VALUES (3024, 24, 1, 19, 0, '2020-12-16 17:08:36', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3025, 27, 2, 15, 5, '2020-12-16 17:27:05', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3026, 37, 1, 17, 1, '2020-12-16 17:28:49', '有货', 1);
INSERT INTO `track_log` VALUES (3027, 37, 1, 17, 0, '2020-12-16 17:31:53', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3028, 30, 1, 19, 1, '2020-12-16 17:43:23', '有货', 1);
INSERT INTO `track_log` VALUES (3029, 30, 1, 19, 0, '2020-12-16 17:48:13', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3030, 24, 2, 15, 5, '2020-12-16 18:00:36', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3031, 35, 1, 17, 1, '2020-12-16 18:10:53', '有货', 1);
INSERT INTO `track_log` VALUES (3032, 35, 1, 17, 0, '2020-12-16 18:13:46', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3033, 31, 1, 19, 1, '2020-12-16 18:29:16', '有货', 1);
INSERT INTO `track_log` VALUES (3034, 31, 1, 19, 0, '2020-12-16 18:35:27', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3035, 43, 1, 17, 1, '2020-12-16 19:01:53', '有货', 1);
INSERT INTO `track_log` VALUES (3036, 43, 1, 17, 0, '2020-12-16 19:07:56', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3037, 19, 1, 19, 1, '2020-12-16 19:15:09', '有货', 1);
INSERT INTO `track_log` VALUES (3038, 19, 1, 19, 0, '2020-12-16 19:20:12', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3039, 40, 1, 17, 1, '2020-12-16 19:50:32', '有货', 1);
INSERT INTO `track_log` VALUES (3040, 40, 1, 17, 0, '2020-12-16 19:54:19', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3041, 32, 1, 19, 1, '2020-12-16 19:55:00', '有货', 1);
INSERT INTO `track_log` VALUES (3042, 32, 1, 19, 0, '2020-12-16 19:58:43', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3043, 34, 1, 17, 0, '2020-12-16 20:46:14', '无货', 1);
INSERT INTO `track_log` VALUES (3044, 25, 1, 19, 1, '2020-12-16 20:50:30', '有货', 1);
INSERT INTO `track_log` VALUES (3045, 25, 1, 19, 0, '2020-12-16 20:58:42', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3046, 42, 1, 15, 0, '2020-12-16 21:25:17', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3047, 39, 1, 18, 0, '2020-12-16 21:34:39', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3048, 21, 1, 19, 1, '2020-12-16 21:53:01', '有货', 1);
INSERT INTO `track_log` VALUES (3049, 38, 1, 17, 0, '2020-12-16 21:56:39', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3050, 21, 1, 19, 0, '2020-12-16 22:02:52', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3051, 41, 1, 16, 0, '2020-12-16 22:07:06', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3052, 33, 1, 19, 1, '2020-12-16 23:04:13', '有货', 1);
INSERT INTO `track_log` VALUES (3053, 23, 1, 17, 0, '2020-12-16 23:05:37', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3054, 33, 1, 19, 0, '2020-12-16 23:08:58', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3055, 29, 1, 17, 0, '2020-12-16 23:12:37', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3056, 22, 1, 19, 0, '2020-12-16 23:39:48', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3057, 21, 2, 19, 5, '2020-12-17 00:30:53', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3058, 26, 1, 17, 0, '2020-12-17 00:31:16', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3059, 22, 2, 19, 5, '2020-12-17 01:13:56', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3060, 27, 1, 17, 1, '2020-12-17 01:14:42', '有货', 1);
INSERT INTO `track_log` VALUES (3061, 27, 1, 17, 0, '2020-12-17 01:15:44', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3062, 23, 2, 19, 5, '2020-12-17 01:45:44', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3063, 24, 1, 17, 1, '2020-12-17 01:59:45', '有货', 1);
INSERT INTO `track_log` VALUES (3064, 24, 1, 17, 0, '2020-12-17 02:05:29', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3065, 27, 2, 19, 3, '2020-12-17 02:08:11', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3066, 28, 1, 17, 0, '2020-12-17 02:41:51', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3067, 42, 2, 15, 3, '2020-12-17 02:45:44', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3068, 42, 2, 15, 3, '2020-12-17 02:45:48', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3069, 21, 1, 17, 1, '2020-12-17 03:20:41', '有货', 1);
INSERT INTO `track_log` VALUES (3070, 21, 1, 17, 0, '2020-12-17 03:26:00', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3071, 29, 2, 19, 5, '2020-12-17 03:56:56', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3072, 22, 1, 17, 1, '2020-12-17 04:18:03', '有货', 1);
INSERT INTO `track_log` VALUES (3073, 22, 1, 17, 0, '2020-12-17 04:29:41', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3074, 24, 2, 19, 5, '2020-12-17 04:54:31', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3075, 23, 1, 17, 1, '2020-12-17 05:15:50', '有货', 1);
INSERT INTO `track_log` VALUES (3076, 23, 1, 17, 0, '2020-12-17 05:23:39', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3077, 22, 2, 19, 5, '2020-12-17 05:39:35', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3078, 27, 1, 17, 0, '2020-12-17 05:50:58', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3079, 28, 2, 19, 5, '2020-12-17 06:35:58', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3080, 29, 1, 17, 0, '2020-12-17 06:37:30', '无货', 1);
INSERT INTO `track_log` VALUES (3081, 32, 2, 16, 4, '2020-12-17 06:39:40', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3082, 20, 1, 19, 0, '2020-12-17 06:51:04', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3083, 26, 1, 19, 1, '2020-12-17 06:54:52', '有货', 1);
INSERT INTO `track_log` VALUES (3084, 27, 2, 17, 5, '2020-12-17 07:03:04', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3085, 26, 1, 19, 0, '2020-12-17 07:16:04', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3086, 19, 1, 17, 0, '2020-12-17 07:26:13', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3087, 36, 2, 15, 4, '2020-12-17 07:30:41', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3088, 20, 2, 19, 5, '2020-12-17 07:44:55', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3089, 39, 2, 15, 4, '2020-12-17 07:46:01', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3090, 25, 1, 17, 0, '2020-12-17 07:54:54', '无库存数据', 1);
INSERT INTO `track_log` VALUES (3091, 19, 2, 19, 5, '2020-12-17 08:25:17', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3092, 25, 2, 19, 5, '2020-12-17 08:58:01', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3093, 26, 2, 19, 4, '2020-12-17 09:53:36', '运输车反馈信号-满', 1);
INSERT INTO `track_log` VALUES (3094, 24, 1, 16, 2, '2020-12-17 10:11:58', '有货', 1);

-- ----------------------------
-- Table structure for warning
-- ----------------------------
DROP TABLE IF EXISTS `warning`;
CREATE TABLE `warning`  (
  `id` int(11) UNSIGNED NULL DEFAULT NULL,
  `area_id` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL,
  `resolve` bit(1) NULL DEFAULT NULL COMMENT '是否解决',
  `dev_id` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `track_id` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道ID',
  `trans_id` int(10) UNSIGNED NOT NULL,
  `content` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `createtime` datetime(0) NULL DEFAULT NULL COMMENT '报警时间',
  `resolvetime` datetime(0) NULL DEFAULT NULL COMMENT '解决时间',
  INDEX `w_createtime_idx`(`createtime`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '报警' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of warning
-- ----------------------------
INSERT INTO `warning` VALUES (6746, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6747, 0, 0, b'1', 2, 0, 0, 'A02: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6748, 0, 0, b'1', 5, 0, 0, 'A05: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6749, 0, 0, b'1', 4, 0, 0, 'A04: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6750, 0, 0, b'1', 8, 0, 0, 'A08: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6751, 0, 0, b'1', 13, 0, 0, 'B05: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:27');
INSERT INTO `warning` VALUES (6752, 0, 0, b'1', 7, 0, 0, 'A07: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6753, 0, 0, b'1', 11, 0, 0, 'B01: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6754, 0, 0, b'1', 15, 0, 0, 'C01: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6755, 0, 0, b'1', 10, 0, 0, 'D02: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6756, 0, 0, b'1', 6, 0, 0, 'A06: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6757, 0, 0, b'1', 9, 0, 0, 'D01: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:26');
INSERT INTO `warning` VALUES (6758, 0, 0, b'1', 18, 0, 0, 'C04: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:27');
INSERT INTO `warning` VALUES (6759, 0, 0, b'1', 19, 0, 0, 'C05: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:33');
INSERT INTO `warning` VALUES (6760, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:27');
INSERT INTO `warning` VALUES (6761, 0, 0, b'1', 16, 0, 0, 'C02: 设备离线', '2020-12-17 09:59:12', '2020-12-17 09:59:27');
INSERT INTO `warning` VALUES (6762, 0, 0, b'1', 3, 0, 0, 'A03: 设备离线', '2020-12-17 09:59:14', '2020-12-17 09:59:22');
INSERT INTO `warning` VALUES (6763, 0, 0, b'1', 17, 0, 0, 'C03: 设备离线', '2020-12-17 09:59:14', '2020-12-17 09:59:22');
INSERT INTO `warning` VALUES (6779, 0, 0, b'1', 5, 0, 0, 'A05: 设备离线', '2020-12-17 10:26:50', '2020-12-17 10:26:50');
INSERT INTO `warning` VALUES (6780, 0, 0, b'1', 6, 0, 0, 'A06: 设备离线', '2020-12-17 10:26:53', '2020-12-17 10:26:53');
INSERT INTO `warning` VALUES (6781, 0, 0, b'1', 8, 0, 0, 'A08: 设备离线', '2020-12-17 10:26:57', '2020-12-17 10:26:57');
INSERT INTO `warning` VALUES (6808, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2020-12-17 12:01:23', '2020-12-17 12:01:39');
INSERT INTO `warning` VALUES (6927, 0, 0, b'1', 9, 0, 0, 'D01: 设备离线', '2020-12-17 20:24:42', '2020-12-17 20:25:31');
INSERT INTO `warning` VALUES (7024, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2020-12-18 03:43:22', '2020-12-18 03:43:37');
INSERT INTO `warning` VALUES (7074, 0, 0, b'1', 9, 0, 0, 'D01: 设备离线', '2020-12-18 06:31:43', '2020-12-18 06:32:27');
INSERT INTO `warning` VALUES (7136, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2020-12-18 10:23:42', '2020-12-18 10:26:23');
INSERT INTO `warning` VALUES (7186, 0, 0, b'1', 15, 0, 0, 'C01: 设备离线', '2020-12-18 13:44:32', '2020-12-18 13:44:32');
INSERT INTO `warning` VALUES (7424, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2020-12-19 14:03:53', '2020-12-19 14:03:53');
INSERT INTO `warning` VALUES (7472, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2020-12-19 16:36:55', '2020-12-19 16:36:55');
INSERT INTO `warning` VALUES (7473, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2020-12-19 16:38:00', '2020-12-19 16:38:00');
INSERT INTO `warning` VALUES (7492, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2020-12-19 17:30:14', '2020-12-19 17:30:14');

-- ----------------------------
-- Table structure for wcs_menu
-- ----------------------------
DROP TABLE IF EXISTS `wcs_menu`;
CREATE TABLE `wcs_menu`  (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `name` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `prior` smallint(3) NULL DEFAULT NULL COMMENT '优先级',
  `memo` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_menu
-- ----------------------------
INSERT INTO `wcs_menu` VALUES (1, '基础菜单', 1, '无特殊权限，通用菜单');
INSERT INTO `wcs_menu` VALUES (2, '管理员菜单', 2, '现场管理员');
INSERT INTO `wcs_menu` VALUES (3, '超级管理员菜单', 3, '超级管理员菜单');
INSERT INTO `wcs_menu` VALUES (4, '原调度菜单', 1, '旧调度菜单配置');

-- ----------------------------
-- Table structure for wcs_menu_dtl
-- ----------------------------
DROP TABLE IF EXISTS `wcs_menu_dtl`;
CREATE TABLE `wcs_menu_dtl`  (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `menu_id` int(6) NULL DEFAULT NULL,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `folder` bit(1) NULL DEFAULT NULL COMMENT '是否是文件夹',
  `folder_id` int(6) NULL DEFAULT NULL COMMENT '所属文件夹ID',
  `module_id` int(6) NULL DEFAULT NULL COMMENT '模块KEY',
  `order` smallint(3) NULL DEFAULT NULL,
  `rf` bit(1) NULL DEFAULT NULL COMMENT '是否是平板的',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 101 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

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
INSERT INTO `wcs_menu_dtl` VALUES (32, 0, '摆渡对位', b'0', 31, 13, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (33, 0, '区域配置', b'0', 31, 2, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (35, 0, '字典', b'0', 31, 29, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (36, 0, '测可入砖', b'0', 31, 9, 4, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (38, 2, '授权', b'1', 0, 0, 6, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (39, 0, '用户', b'0', 38, 31, 1, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (40, 0, '菜单', b'0', 38, 30, 2, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (41, 3, '任务', b'1', 0, 0, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (42, 0, '任务开关', b'0', 41, 3, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (43, 0, '任务状态', b'0', 41, 11, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (44, 3, '设备', b'1', 0, 0, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (45, 0, '砖机', b'0', 44, 6, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (46, 0, '摆渡车', b'0', 44, 4, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (47, 0, '运输车', b'0', 44, 5, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (48, 3, '轨道', b'1', 0, 0, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (49, 0, '轨道状态', b'0', 48, 7, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (50, 0, '库存修改', b'0', 48, 10, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (51, 3, '统计', b'1', 0, 0, 4, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (52, 0, '轨道库存', b'0', 51, 15, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (53, 0, '警告信息', b'0', 51, 16, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (54, 0, '空满轨道', b'0', 51, 17, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (55, 3, '配置', b'1', 0, 0, 5, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (56, 0, '摆渡对位', b'0', 55, 13, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (57, 0, '区域配置', b'0', 55, 2, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (58, 0, '轨道分配', b'0', 55, 14, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (59, 0, '字典', b'0', 55, 29, 4, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (60, 0, '测可入砖', b'0', 55, 9, 5, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (61, 0, '添加任务', b'0', 55, 12, 6, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (62, 3, '授权', b'1', 0, 0, 6, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (63, 0, '用户', b'0', 62, 31, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (64, 0, '菜单', b'0', 62, 30, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (65, 4, '主页', b'0', NULL, 1, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (66, 4, '任务', b'1', NULL, NULL, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (67, NULL, '开关', b'0', 66, 3, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (68, NULL, '任务', b'0', 66, 11, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (69, NULL, '按轨出库', b'0', 66, 18, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (70, 4, '设备', b'1', NULL, NULL, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (71, NULL, '砖机', b'0', 70, 6, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (72, NULL, '摆渡车', b'0', 70, 4, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (73, NULL, '运输车', b'0', 70, 5, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (74, NULL, '轨道', b'0', 70, 7, 4, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (75, 4, '统计', b'1', NULL, NULL, 4, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (76, NULL, '规格', b'0', 75, 8, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (77, NULL, '库存', b'0', 75, 15, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (78, NULL, '轨道', b'0', 75, 10, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (79, 4, '设置', b'1', NULL, NULL, 5, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (80, NULL, '轨道分配', b'0', 79, 14, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (81, NULL, '摆渡对位', b'0', 79, 13, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (82, NULL, '区域配置', b'0', 79, 2, 3, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (83, NULL, '字典', b'0', 79, 29, 4, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (84, NULL, '测可入砖', b'0', 79, 9, 5, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (85, 4, '记录', b'1', NULL, NULL, 6, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (86, NULL, '警告', b'0', 85, 16, 1, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (87, NULL, '空满轨道', b'0', 85, 17, 2, NULL);
INSERT INTO `wcs_menu_dtl` VALUES (88, 0, '设备及轨道', b'0', 15, 2, 3, b'0');
INSERT INTO `wcs_menu_dtl` VALUES (89, 0, '规格编号', b'0', 15, 8, 1, b'0');
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

-- ----------------------------
-- Table structure for wcs_module
-- ----------------------------
DROP TABLE IF EXISTS `wcs_module`;
CREATE TABLE `wcs_module`  (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '模块名称',
  `type` tinyint(3) NULL DEFAULT NULL COMMENT '类型：PC, RF, 电视..',
  `key` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '模块对应界面key',
  `entity` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '平板-模块类名',
  `brush` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `geometry` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT 'Tab图标',
  `winctlname` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT 'PC调度界面文件名',
  `memo` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 33 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

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
INSERT INTO `wcs_module` VALUES (8, '规格', 0, 'Goods', '', 'DarkPrimaryBrush', 'ConfigGeometry', 'GoodsCtl', '规格详细信息');
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
INSERT INTO `wcs_module` VALUES (21, '砖机规格', 1, 'RFTILEGOOD', 'com.keda.wcsfixplatformapp.screen.rftilegood.RfTileGoodScreen', '', 'updowndev.png', '', '平板-砖机规格查看/修改');
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

-- ----------------------------
-- Table structure for wcs_role
-- ----------------------------
DROP TABLE IF EXISTS `wcs_role`;
CREATE TABLE `wcs_role`  (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `name` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '角色名称',
  `admin` bit(1) NULL DEFAULT NULL COMMENT '是否是管理员角色',
  `menu_id` int(6) NULL DEFAULT NULL COMMENT '菜单',
  `prior` tinyint(3) NULL DEFAULT NULL COMMENT '优先级',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_role
-- ----------------------------
INSERT INTO `wcs_role` VALUES (1, '普通角色', b'0', 1, 1);
INSERT INTO `wcs_role` VALUES (2, '管理人员', b'0', 2, 2);
INSERT INTO `wcs_role` VALUES (3, '超级管理员', b'1', 3, 100);

-- ----------------------------
-- Table structure for wcs_user
-- ----------------------------
DROP TABLE IF EXISTS `wcs_user`;
CREATE TABLE `wcs_user`  (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `username` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '用户名',
  `password` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `memo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `role_id` int(6) NULL DEFAULT NULL COMMENT '角色ID',
  `exitwcs` bit(1) NULL DEFAULT NULL COMMENT '能否退出调度',
  `guest` bit(1) NULL DEFAULT NULL COMMENT '默认登陆用户',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_user
-- ----------------------------
INSERT INTO `wcs_user` VALUES (1, '123', '123', '金凯瑞', '一般操作', 1, b'1', b'1');
INSERT INTO `wcs_user` VALUES (2, 'admin', 'admin', '管理员', '', 2, b'1', b'0');
INSERT INTO `wcs_user` VALUES (3, 'supervisor', 'supervisor', '超级管理员', '', 100, b'1', b'0');

-- ----------------------------
-- View structure for active_dev
-- ----------------------------
DROP VIEW IF EXISTS `active_dev`;
CREATE ALGORITHM = UNDEFINED DEFINER = `root`@`localhost` SQL SECURITY DEFINER VIEW `active_dev` AS select `t`.`id` AS `id`,`t`.`name` AS `name`,`t`.`ip` AS `ip`,`t`.`port` AS `port`,`t`.`type` AS `type`,`t`.`type2` AS `type2`,`t`.`enable` AS `enable`,`t`.`att1` AS `att1`,`t`.`att2` AS `att2`,`t`.`goods_id` AS `goods_id`,`t`.`left_track_id` AS `left_track_id`,`t`.`right_track_id` AS `right_track_id`,`t`.`brother_dev_id` AS `brother_dev_id`,`t`.`strategy_in` AS `strategy_in`,`t`.`strategy_out` AS `strategy_out`,`t`.`memo` AS `memo` from `device` `t` where (`t`.`enable` = 1);

-- ----------------------------
-- View structure for stock_sum
-- ----------------------------
DROP VIEW IF EXISTS `stock_sum`;
CREATE ALGORITHM = UNDEFINED DEFINER = `root`@`localhost` SQL SECURITY DEFINER VIEW `stock_sum` AS select `t`.`goods_id` AS `goods_id`,`t`.`track_id` AS `track_id`,min(`t`.`produce_time`) AS `produce_time`,count(`t`.`id`) AS `count`,sum(`t`.`pieces`) AS `pieces`,sum(`t`.`stack`) AS `stack`,`t`.`area` AS `area`,`t`.`track_type` AS `track_type` from `stock` `t` where (`t`.`track_type` in (2,3,4)) group by `t`.`track_id`,`t`.`goods_id` order by `t`.`area`,`t`.`goods_id`,`produce_time`,`t`.`track_id`;

-- ----------------------------
-- Procedure structure for DELETE_DATA
-- ----------------------------
DROP PROCEDURE IF EXISTS `DELETE_DATA`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DELETE_DATA`()
BEGIN
	/*仅保留 31 天的报警数据*/
	delete from warning where resolve = 1 and DATEDIFF(CURRENT_DATE,createtime) >= 31;
	/*仅保留 31 天的任务数据*/
	delete from stock_trans where (finish = 1 or cancel = 1) and DATEDIFF(CURRENT_DATE,create_time) >= 31;
	/*仅保留 31 天的记录数据*/
	delete from stock_log where DATEDIFF(CURRENT_DATE,create_time) >= 31;
	delete from track_log where DATEDIFF(CURRENT_DATE,log_time) >= 31;
END
;;
delimiter ;

-- ----------------------------
-- Event structure for DELETE_EVEN
-- ----------------------------
DROP EVENT IF EXISTS `DELETE_EVEN`;
delimiter ;;
CREATE DEFINER = `root`@`localhost` EVENT `DELETE_EVEN`
ON SCHEDULE
EVERY '1' DAY STARTS '2020-10-02 01:00:00'
DO CALL DELETE_DATA()
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
