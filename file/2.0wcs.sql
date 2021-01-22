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

 Date: 22/01/2021 17:32:50
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
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area
-- ----------------------------
INSERT INTO `area` VALUES (1, '包装前#', b'1', b'1', '包装前', 3, 0, 5);
INSERT INTO `area` VALUES (2, '窑后#', b'1', b'1', '窑后', 3, 0, 5);

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
) ENGINE = InnoDB AUTO_INCREMENT = 20 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_device
-- ----------------------------
INSERT INTO `area_device` VALUES (1, 1, 1, 6, NULL);
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
  `id` int(11) UNSIGNED NOT NULL COMMENT '标识',
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
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 51 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

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
-- Table structure for config_carrier
-- ----------------------------
DROP TABLE IF EXISTS `config_carrier`;
CREATE TABLE `config_carrier`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '运输车设备ID',
  `a_takemisstrack` bit(1) NULL DEFAULT NULL COMMENT '后退取货扫不到点',
  `a_givemisstrack` bit(1) NULL DEFAULT NULL COMMENT '前进放货扫不到点',
  `a_alert_track` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '所在轨道',
  `stock_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '库存ID',
  `length` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '运输顶板长度（脉冲）',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `carrier_stock_id_index`(`stock_id`) USING BTREE,
  CONSTRAINT `carrier_id_fk` FOREIGN KEY (`id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `carrier_stock_id_fk` FOREIGN KEY (`stock_id`) REFERENCES `stock` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config_carrier
-- ----------------------------
INSERT INTO `config_carrier` VALUES (15, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `config_carrier` VALUES (16, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `config_carrier` VALUES (17, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `config_carrier` VALUES (18, NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for config_ferry
-- ----------------------------
DROP TABLE IF EXISTS `config_ferry`;
CREATE TABLE `config_ferry`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '摆渡车设备ID',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车轨道ID',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `ferry_track_id_index`(`track_id`) USING BTREE,
  CONSTRAINT `ferry__id_fk` FOREIGN KEY (`id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `ferry_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config_ferry
-- ----------------------------
INSERT INTO `config_ferry` VALUES (11, 45);
INSERT INTO `config_ferry` VALUES (12, 46);
INSERT INTO `config_ferry` VALUES (13, 47);

-- ----------------------------
-- Table structure for config_tilelifter
-- ----------------------------
DROP TABLE IF EXISTS `config_tilelifter`;
CREATE TABLE `config_tilelifter`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '砖机设备ID',
  `brother_dev_id` int(10) UNSIGNED NULL DEFAULT NULL COMMENT '前设备ID[兄弟砖机ID]',
  `left_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上下砖机：左轨道ID',
  `right_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上下砖机：右轨道ID',
  `strategy_in` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '下砖策略',
  `strategy_out` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '上砖策略',
  `work_type` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '作业类型\r\n砖机：0按规格 1按轨道',
  `last_track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '砖机上次作业轨道',
  `old_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '上一个品种',
  `goods_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '品种ID',
  `pre_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '预设品种',
  `do_shift` bit(1) NULL DEFAULT NULL COMMENT '开启转产',
  `can_cutover` bit(1) NULL DEFAULT NULL COMMENT '可切换模式',
  `work_mode` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '作业模式\r\n0：过砖模式\r\n1：上砖模式\r\n2：下砖模式',
  `work_mode_next` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '下一个作业模式',
  `do_cutover` bit(1) NULL DEFAULT NULL COMMENT '开启切换模式',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `tile_goods_id_index`(`goods_id`) USING BTREE,
  INDEX `tile_ltrack_id_index`(`left_track_id`) USING BTREE,
  INDEX `tile_rtrack_id_index`(`right_track_id`) USING BTREE,
  CONSTRAINT `tile_goods_id_fk` FOREIGN KEY (`goods_id`) REFERENCES `goods` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `tile_id_fk` FOREIGN KEY (`id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `tile_ltrack_id_fk` FOREIGN KEY (`left_track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `tile_rtrack_id_fk` FOREIGN KEY (`right_track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config_tilelifter
-- ----------------------------
INSERT INTO `config_tilelifter` VALUES (1, 0, 1, 2, 4, 0, 0, 0, 0, 198, 36, b'0', b'1', 2, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (2, 0, 3, 4, 4, 0, 0, 0, 0, 193, 0, b'0', b'0', 0, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (3, 0, 5, 6, 4, 0, 0, 0, 0, 192, 0, b'0', b'0', 0, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (4, 0, 7, 8, 4, 0, 0, 0, 0, 192, 0, b'0', b'0', 0, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (5, 0, 9, 10, 4, 0, 0, 0, 0, 194, 0, b'0', b'0', 0, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (6, 0, 11, 12, 4, 0, 0, 0, 0, 194, 0, b'0', b'0', 0, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (7, 0, 13, 14, 4, 0, 0, 0, 0, 195, 0, b'0', b'0', 0, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (8, 0, 15, 16, 4, 0, 0, 0, 0, 194, 0, b'0', b'0', 0, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (9, 0, 17, 18, 0, 1, 0, 0, 0, 192, 0, b'0', b'0', 0, 0, b'0');
INSERT INTO `config_tilelifter` VALUES (10, 0, 49, 50, 0, 1, 0, 0, 0, 188, 0, b'0', b'0', 0, 0, b'0');

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
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '\'wcs_yme_yh.active_dev\' is not BASE TABLE' ROW_FORMAT = Dynamic;

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
  `do_work` bit(1) NULL DEFAULT NULL COMMENT '开启作业',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 35 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of device
-- ----------------------------
INSERT INTO `device` VALUES (1, 'A01', '192.168.0.31', 2000, 6, 2, b'0', 0, 0, '161', 1, b'1');
INSERT INTO `device` VALUES (2, 'A02', '192.168.0.36', 2000, 1, 2, b'0', 0, 0, '162', 1, b'1');
INSERT INTO `device` VALUES (3, 'A03', '192.168.0.41', 2000, 1, 2, b'0', 0, 0, '163', 1, b'1');
INSERT INTO `device` VALUES (4, 'A04', '192.168.0.46', 2000, 1, 2, b'0', 0, 0, '164', 1, b'1');
INSERT INTO `device` VALUES (5, 'A05', '192.168.0.51', 2000, 1, 2, b'0', 0, 0, '165', 1, b'1');
INSERT INTO `device` VALUES (6, 'A06', '192.168.0.56', 2000, 1, 2, b'0', 0, 0, '166', 1, b'1');
INSERT INTO `device` VALUES (7, 'A07', '192.168.0.61', 2000, 1, 2, b'0', 0, 0, '167', 1, b'1');
INSERT INTO `device` VALUES (8, 'A08', '192.168.0.66', 2000, 1, 2, b'0', 0, 0, '168', 1, b'1');
INSERT INTO `device` VALUES (9, 'D01', '192.168.0.81', 2000, 0, 2, b'0', 0, 0, '209', 1, b'1');
INSERT INTO `device` VALUES (10, 'D02', '192.168.0.86', 2000, 0, 2, b'0', 0, 0, '210', 1, b'1');
INSERT INTO `device` VALUES (11, 'B01', '192.168.0.131', 2000, 3, 0, b'0', 0, 0, '177', 1, b'1');
INSERT INTO `device` VALUES (12, 'B02', '192.168.0.132', 2000, 3, 0, b'0', 0, 0, '178', 1, b'1');
INSERT INTO `device` VALUES (13, 'B05', '192.168.0.135', 2000, 2, 0, b'0', 0, 0, '181', 1, b'1');
INSERT INTO `device` VALUES (15, 'C01', '127.0.0.1', 2003, 4, 0, b'0', 0, 0, '193', 1, b'1');
INSERT INTO `device` VALUES (16, 'C02', '192.168.0.152', 2000, 4, 0, b'0', 0, 0, '194', 1, b'1');
INSERT INTO `device` VALUES (17, 'C03', '192.168.0.153', 2000, 4, 0, b'0', 0, 0, '195', 1, b'1');
INSERT INTO `device` VALUES (18, 'C04', '192.168.0.154', 2000, 4, 0, b'0', 0, 0, '196', 1, b'1');
INSERT INTO `device` VALUES (19, 'C05', '192.168.0.155', 2000, 4, 0, b'0', 0, 0, '197', 1, b'1');

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
INSERT INTO `diction` VALUES (2, 0, 1, '任务开关', b'1', b'1', b'1', 1);
INSERT INTO `diction` VALUES (3, 0, 2, '警告信息', b'1', b'1', b'1', 1);
INSERT INTO `diction` VALUES (4, 0, 0, '最小存放时间', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (5, 0, 0, '摆渡车安全距离', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (6, 0, 0, '版本信息', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (7, 0, 0, '转产差值', b'0', b'1', b'0', 1);
INSERT INTO `diction` VALUES (8, 0, 1, '配置开关', b'0', b'1', b'0', 100);
INSERT INTO `diction` VALUES (10, 0, 0, '等级字典', b'1', b'1', b'0', 100);
INSERT INTO `diction` VALUES (11, 0, 3, '运输车脉冲值转换', b'1', b'1', b'1', 10);

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
INSERT INTO `diction_dtl` VALUES (1, 1, 'NewStockId', '生成库存ID', NULL, NULL, '', NULL, 5328, NULL, '2021-01-21 19:54:45');
INSERT INTO `diction_dtl` VALUES (2, 1, 'NewTranId', '生成交易ID', NULL, NULL, '', NULL, 8294, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (3, 1, 'NewWarnId', '生成警告ID', NULL, NULL, '', NULL, 6848, NULL, '2021-01-20 08:18:24');
INSERT INTO `diction_dtl` VALUES (4, 1, 'NewGoodId', '生成品种ID', NULL, NULL, '', NULL, 199, NULL, '2021-01-18 15:05:39');
INSERT INTO `diction_dtl` VALUES (5, 2, 'Area1Down', '1号线下砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (6, 2, 'Area1Up', '1号线上砖', NULL, b'0', '', NULL, NULL, NULL, NULL);
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
INSERT INTO `diction_dtl` VALUES (52, 4, 'MinStockTime', '最小存放时间(小时)', 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (53, 5, 'FerryAvoidNumber', '摆渡车(轨道数)', 3, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (54, 3, 'UpTileHaveNoTrackToOut', '砖机找不到合适轨道上砖', NULL, NULL, '砖机找不到合适轨道上砖', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (57, 6, 'PDA_INIT_VERSION', 'PDA基础字典版本', 9, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (58, 6, 'PDA_GOOD_VERSION', 'PDA规格字典版本', 9, NULL, '', NULL, NULL, NULL, '2021-01-18 15:05:39');
INSERT INTO `diction_dtl` VALUES (59, 7, 'TileLifterShiftCount', '下砖机转产差值(层数)', 99, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (60, 8, 'UserLoginFunction', 'PDA登陆功能开关', NULL, b'1', 'PDA登陆功能开关', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (61, 3, 'TileGoodsIsZero', '砖机工位品种反馈异常', NULL, NULL, '砖机工位品种反馈异常', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (62, 3, 'TileGoodsIsNull', '砖机工位品种无数据', NULL, NULL, '砖机工位品种无数据', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (101, 10, 'GoodLevel', '优等品', 1, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (102, 10, 'GoodLevel', '一级品', 2, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (103, 10, 'GoodLevel', 'A', 3, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (104, 10, 'GoodLevel', 'B', 4, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (105, 10, 'GoodLevel', 'C', 5, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (106, 10, 'GoodLevel', 'D', 6, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (107, 10, 'GoodLevel', 'E', 7, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (108, 10, 'GoodLevel', 'F', 8, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (109, 10, 'GoodLevel', 'G', 9, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (110, 11, 'Pulse2CM', '1脉冲=厘米', 0, b'0', '', 1.736, 0, 0, '2021-01-21 17:39:15');

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
) ENGINE = InnoDB AUTO_INCREMENT = 155 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

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
-- Table structure for good_size
-- ----------------------------
DROP TABLE IF EXISTS `good_size`;
CREATE TABLE `good_size`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '规格ID',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `length` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '长',
  `width` smallint(5) NULL DEFAULT NULL COMMENT '宽',
  `stack` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '垛',
  `car_lenght` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '一车砖的长度（脉冲）',
  `car_space` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '砖与砖安全间距（脉冲）',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 38 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of good_size
-- ----------------------------
INSERT INTO `good_size` VALUES (1, '600x600', 600, 600, 6, NULL, NULL);
INSERT INTO `good_size` VALUES (2, '800x400', 800, 400, 8, NULL, NULL);
INSERT INTO `good_size` VALUES (3, '800x400', 800, 400, 5, NULL, NULL);
INSERT INTO `good_size` VALUES (4, '800x800', 800, 800, 4, NULL, NULL);
INSERT INTO `good_size` VALUES (5, '800x2600', 2600, 800, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (6, '1200x2400', 2400, 1200, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (7, '1200x2700', 2700, 1200, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (8, '1600x3200', 3200, 1600, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (9, '1200x2400', 2400, 1200, 3, NULL, NULL);
INSERT INTO `good_size` VALUES (10, '600x600', 600, 600, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (11, '600x900', 900, 600, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (12, '600x1200', 1200, 600, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (13, '700x1300', 1300, 700, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (14, '750x1500', 1500, 750, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (15, '800x1400', 1400, 800, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (16, '800x1600', 1600, 800, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (17, '800x2000', 2000, 800, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (18, '700x1300', 1300, 700, 3, NULL, NULL);
INSERT INTO `good_size` VALUES (19, '750x1500', 1500, 750, 3, NULL, NULL);
INSERT INTO `good_size` VALUES (20, '900x1800', 1800, 900, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (21, '800x2000', 2000, 800, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (22, '800x2600', 2600, 800, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (23, '1200x2400', 2400, 1200, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (24, '1200x2700', 2700, 1200, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (25, '1600x3200', 3200, 1600, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (26, '1200x2400', 2400, 1200, 3, NULL, NULL);
INSERT INTO `good_size` VALUES (27, '1200x2700', 2700, 1200, 3, NULL, NULL);
INSERT INTO `good_size` VALUES (28, '1000x3000', 3000, 1000, 3, NULL, NULL);
INSERT INTO `good_size` VALUES (29, '800x2600', 2600, 800, 3, NULL, NULL);
INSERT INTO `good_size` VALUES (30, '600x700', 700, 600, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (31, '800x800', 800, 800, 4, NULL, NULL);
INSERT INTO `good_size` VALUES (32, '600x600', 600, 600, 4, NULL, NULL);
INSERT INTO `good_size` VALUES (33, '1600x800', 800, 1600, 1, NULL, NULL);
INSERT INTO `good_size` VALUES (34, '1400x800', 800, 1400, 2, NULL, NULL);
INSERT INTO `good_size` VALUES (35, '800x800', 800, 800, 6, NULL, NULL);
INSERT INTO `good_size` VALUES (36, '600x1200', 1200, 600, 4, NULL, NULL);
INSERT INTO `good_size` VALUES (37, '2000x1200', 1200, 2000, 3, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 199 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of goods
-- ----------------------------
INSERT INTO `goods` VALUES (33, 1, '881062', '881062', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (35, 1, '88103', '88103', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (36, 1, '8208', '8208', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (37, 1, '88307', '88307', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (38, 1, '88306', '88306', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (39, 1, '88711', '88711', 51, 0, '', NULL, NULL, 2, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (40, 1, '88112', '88112', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (49, 1, '1:800x800-88116', '88116', 51, 0, '', '2020-12-15 09:32:33', NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (50, 1, '88116一级', '88116一级', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (51, 1, '88311', '88311', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (60, 1, '3H092色', '3H092色', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (61, 1, '5833', '5833', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (66, 1, '5833二色', '5833二色', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (72, 1, '88311二色', '88311二色', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (76, 1, '8106   3色', '8106  3色', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (87, 1, '8812', '8812', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (100, 1, '88112L2色', '88112L2色', 51, 0, '', NULL, NULL, 4, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (113, 1, '88108  1色', '88108  1色', 51, 0, '', NULL, NULL, 4, 0, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (164, 1, '6802一级', '6802一级', 51, 0, '', NULL, NULL, 4, 2, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (174, 1, '6859   一色', '6859   一色', 51, 0, '', NULL, NULL, 4, 2, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (175, 1, '6859      一级', '6859    一级', 51, 0, '', NULL, NULL, 4, 2, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (176, 1, '6805一色', '6805一色', 51, 0, '', NULL, NULL, 3, 2, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (177, 1, '6805一级', '6805一级', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (178, 1, '6808一色', '6808一色', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (180, 1, '6805二色', '6805二色', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (181, 1, '88306一色', '88306一色', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (182, 1, '88306一级', '88306一级', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (183, 1, '6859一色', '6859一色', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (185, 1, '6808二色一级', '6808二色一级', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (186, 1, '12702  一色', '12702', 54, 0, '', '2021-01-09 09:05:43', NULL, 1, 1, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (187, 1, '12702一级', '12702一级', 54, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (188, 1, '1.2*800-12702 一色', '1.2*800-12702 一色', 54, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (192, 1, '88702     一色', '88702   一色', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (193, 1, '88702    一级', '88702    一级', 51, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (194, 1, '1.2*600-12702  二色', '1.2*600-12702  二色', 54, 0, '', NULL, NULL, 3, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (195, 1, '1.2*600-12702  二色一级', '1.2*600-12702  二色一级', 54, 0, '', NULL, NULL, 5, 3, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (196, 1, '1.2*600-12702  一色', '1.2*600-12702  一色', 54, 0, '', NULL, NULL, 5, 5, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (197, 1, '1.2*600-12702  一色一级', '1.2*600-12702  一色一级', 54, 0, '', NULL, NULL, 5, 5, NULL, NULL, NULL, NULL);
INSERT INTO `goods` VALUES (198, 1, 'ABC', '666', 50, 0, '', '2021-01-18 15:05:39', NULL, 4, 3, '2021-01-18 15:05:39', NULL, NULL, 'ABC/666/800x800/A');

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
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

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
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `sto_goods_id_fk`(`goods_id`) USING BTREE,
  INDEX `sto_track_id_fk`(`track_id`) USING BTREE,
  CONSTRAINT `sto_goods_id_fk` FOREIGN KEY (`goods_id`) REFERENCES `goods` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `sto_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5328 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock
-- ----------------------------
INSERT INTO `stock` VALUES (833, 33, 4, 204, 47, '2020-12-02 16:03:50', 2, 0, 7, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (893, 35, 4, 204, 45, '2020-12-02 19:26:10', 2, 1, 2, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (958, 36, 4, 204, 46, '2020-12-03 03:21:31', 0, 0, 7, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (960, 36, 4, 204, 46, '2020-12-03 03:41:49', 0, 0, 5, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (961, 36, 4, 204, 46, '2020-12-03 03:50:05', 0, 0, 6, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (981, 36, 4, 200, 46, '2020-12-03 05:22:09', 0, 0, 8, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (1065, 36, 4, 204, 46, '2020-12-03 14:57:47', 0, 0, 5, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (1091, 37, 4, 200, 46, '2020-12-03 17:53:53', 1, 1, 8, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (1121, 37, 4, 204, 47, '2020-12-03 20:24:10', 0, 0, 7, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (1123, 35, 4, 204, 47, '2020-12-03 20:30:59', 1, 0, 3, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (1150, 35, 4, 204, 47, '2020-12-03 22:40:29', 0, 0, 0, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (1418, 39, 4, 204, 46, '2020-12-04 21:52:20', 0, 0, 5, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (1462, 39, 4, 204, 46, '2020-12-05 01:28:55', 0, 0, 7, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (1463, 38, 4, 204, 45, '2020-12-05 01:37:06', 0, 0, 1, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (1622, 38, 4, 204, 47, '2020-12-05 13:47:04', 0, 0, 1, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (1818, 40, 4, 204, 47, '2020-12-06 04:36:08', 1, 0, 2, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (1899, 51, 4, 204, 47, '2020-12-06 12:00:41', 1, 0, 1, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (1941, 51, 4, 204, 7, '2020-12-06 14:38:09', 0, 0, 4, 1, 1, NULL, NULL);
INSERT INTO `stock` VALUES (2041, 49, 4, 204, 46, '2020-12-06 22:01:23', 0, 0, 6, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (2042, 50, 4, 204, 45, '2020-12-06 22:13:22', 0, 0, 7, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (2296, 60, 4, 204, 45, '2020-12-07 16:57:50', 0, 0, 1, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (2327, 61, 4, 200, 46, '2020-12-07 18:45:39', 1, 1, 8, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (2365, 66, 4, 204, 46, '2020-12-07 20:51:18', 0, 0, 6, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (2501, 72, 4, 204, 47, '2020-12-08 07:48:52', 1, 0, 1, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (2502, 72, 4, 204, 47, '2020-12-08 08:04:52', 2, 0, 3, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (2543, 76, 4, 204, 45, '2020-12-08 11:29:18', 0, 0, 1, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (2693, 87, 4, 204, 45, '2020-12-09 01:23:56', 0, 0, 4, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (2856, 100, 4, 200, 46, '2020-12-09 15:32:17', 0, 0, 8, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (3233, 113, 4, 204, 46, '2020-12-10 17:53:17', 0, 0, 6, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (5172, 180, 4, 204, 46, '2020-12-16 16:23:46', 0, 0, 6, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (5189, 181, 4, 200, 46, '2020-12-16 18:34:43', 1, 1, 8, 1, 5, NULL, NULL);
INSERT INTO `stock` VALUES (5207, 197, 3, 162, 42, '2020-12-17 06:32:00', 0, 0, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5209, 197, 3, 162, 42, '2020-12-17 06:32:00', 1, 1, 5, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5211, 197, 3, 162, 42, '2020-12-17 06:32:00', 2, 1, 6, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5212, 197, 3, 162, 40, '2020-12-17 06:32:00', 0, 0, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5215, 197, 3, 162, 40, '2020-12-17 06:32:00', 1, 1, 6, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5218, 188, 3, 162, 49, '2020-12-17 03:10:00', 0, 0, 8, 1, 0, NULL, NULL);
INSERT INTO `stock` VALUES (5219, 188, 3, 162, 37, '2020-12-17 03:10:00', 0, 0, 7, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5222, 188, 3, 162, 50, '2020-12-17 03:10:00', 1, 0, 6, 1, 0, NULL, NULL);
INSERT INTO `stock` VALUES (5223, 188, 3, 162, 38, '2020-12-17 03:10:00', 2, 0, 5, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5225, 188, 3, 162, 37, '2020-12-17 03:10:00', 1, 1, 7, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5226, 188, 3, 162, 38, '2020-12-17 03:10:00', 3, 1, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5228, 197, 3, 162, 42, '2020-12-17 06:32:00', 3, 1, 0, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5229, 188, 3, 162, 35, '2020-12-17 03:11:00', 0, 0, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5230, 188, 3, 162, 37, '2020-12-17 03:10:00', 2, 1, 7, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5231, 188, 3, 162, 35, '2020-12-17 03:11:00', 1, 1, 6, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5232, 188, 3, 162, 33, '2020-12-17 03:12:15', 0, 0, 5, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5233, 188, 3, 162, 33, '2020-12-17 03:22:32', 1, 1, 5, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5240, 194, 3, 162, 31, '2020-12-17 06:18:00', 0, 0, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5241, 194, 3, 162, 31, '2020-12-17 06:18:00', 1, 1, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5243, 195, 3, 162, 47, '2020-12-17 06:18:00', 1, 0, 7, 1, 6, NULL, NULL);
INSERT INTO `stock` VALUES (5245, 192, 4, 204, 18, '2020-12-17 04:38:05', 2, 0, 3, 1, 0, NULL, NULL);
INSERT INTO `stock` VALUES (5247, 192, 4, 204, 24, '2020-12-17 04:42:12', 3, 0, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5248, 194, 3, 162, 31, '2020-12-17 06:18:00', 2, 1, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5249, 192, 4, 204, 24, '2020-12-17 04:49:21', 4, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5250, 194, 3, 162, 31, '2020-12-17 06:18:00', 3, 1, 5, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5251, 192, 4, 204, 22, '2020-12-17 05:05:39', 0, 0, 4, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5252, 192, 4, 204, 22, '2020-12-17 05:11:01', 1, 1, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5254, 192, 4, 204, 22, '2020-12-17 05:14:50', 2, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5255, 192, 4, 204, 22, '2020-12-17 05:18:06', 3, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5256, 192, 4, 204, 22, '2020-12-17 05:35:46', 4, 1, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5257, 193, 4, 204, 23, '2020-12-17 05:35:49', 0, 0, 2, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5258, 192, 4, 204, 28, '2020-12-17 05:43:59', 0, 0, 4, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5259, 194, 3, 162, 32, '2020-12-17 06:17:00', 1, 1, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5260, 192, 4, 204, 28, '2020-12-17 05:45:38', 1, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5261, 194, 3, 162, 32, '2020-12-17 06:17:00', 0, 0, 0, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5262, 194, 3, 162, 32, '2020-12-17 06:17:00', 2, 1, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5263, 192, 4, 204, 28, '2020-12-17 06:09:22', 2, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5264, 195, 3, 162, 36, '2020-12-17 06:18:00', 1, 0, 7, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5265, 192, 4, 204, 28, '2020-12-17 06:28:42', 3, 1, 4, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5266, 192, 4, 204, 28, '2020-12-17 06:32:37', 4, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5267, 194, 3, 162, 32, '2020-12-17 06:35:22', 3, 1, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5268, 192, 4, 204, 27, '2020-12-17 06:35:59', 0, 0, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5269, 194, 3, 162, 39, '2020-12-17 06:40:35', 1, 0, 5, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5270, 192, 4, 204, 27, '2020-12-17 06:40:59', 1, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5272, 192, 4, 204, 27, '2020-12-17 06:51:45', 2, 1, 4, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5273, 195, 3, 162, 36, '2020-12-17 06:53:57', 2, 1, 7, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5274, 192, 4, 204, 27, '2020-12-17 06:55:51', 3, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5275, 192, 4, 204, 27, '2020-12-17 06:59:34', 4, 1, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5277, 194, 3, 162, 39, '2020-12-17 07:16:33', 1, 1, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5278, 194, 3, 162, 39, '2020-12-17 07:21:28', 2, 1, 6, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5280, 195, 3, 162, 36, '2020-12-17 07:26:59', 3, 1, 7, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5281, 192, 4, 204, 20, '2020-12-17 07:32:30', 3, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5284, 194, 3, 162, 39, '2020-12-17 07:42:43', 3, 1, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5285, 192, 4, 204, 19, '2020-12-17 07:54:47', 0, 0, 4, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5286, 192, 4, 204, 19, '2020-12-17 07:59:31', 1, 1, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5289, 192, 4, 204, 19, '2020-12-17 08:04:27', 2, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5290, 185, 4, 204, 21, '2020-12-17 07:58:00', 0, 0, 0, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5291, 192, 4, 204, 19, '2020-12-17 08:07:39', 3, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5294, 196, 3, 162, 44, '2020-12-17 08:16:51', 1, 0, 8, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5295, 192, 4, 204, 19, '2020-12-17 08:20:48', 4, 1, 4, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5296, 192, 4, 204, 25, '2020-12-17 08:25:20', 0, 0, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5297, 192, 4, 204, 25, '2020-12-17 08:29:13', 1, 1, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5298, 192, 4, 204, 25, '2020-12-17 08:33:57', 2, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5300, 192, 4, 204, 25, '2020-12-17 08:51:14', 3, 1, 4, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5301, 192, 4, 204, 25, '2020-12-17 08:54:39', 4, 1, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5302, 192, 4, 204, 26, '2020-12-17 08:58:04', 0, 0, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5303, 192, 4, 204, 26, '2020-12-17 09:12:23', 1, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5304, 193, 4, 204, 23, '2020-12-17 09:20:36', 1, 1, 2, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5307, 192, 4, 204, 26, '2020-12-17 09:43:37', 2, 1, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5308, 192, 4, 204, 26, '2020-12-17 09:47:04', 3, 1, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5309, 192, 4, 204, 8, '2020-12-17 09:49:54', 0, 0, 4, 1, 1, NULL, NULL);
INSERT INTO `stock` VALUES (5310, 196, 3, 162, 44, '2020-12-17 09:50:00', 1, 1, 0, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5311, 192, 4, 204, 29, '2020-12-17 09:54:55', 0, 0, 1, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5317, 192, 4, 204, 29, '2020-12-17 10:02:47', 1, 1, 4, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5318, 192, 4, 204, 29, '2020-12-17 10:06:13', 2, 1, 3, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5319, 194, 3, 162, 15, '2020-12-17 10:11:26', 0, 0, 8, 1, 1, NULL, NULL);
INSERT INTO `stock` VALUES (5320, 192, 4, 204, 1, '2020-12-17 10:11:46', 0, 0, 1, 1, 1, NULL, NULL);
INSERT INTO `stock` VALUES (5321, 33, 4, 204, 19, '2020-12-31 16:59:38', 5, 1, 0, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5322, 49, 4, 204, 19, '2020-12-31 16:59:48', 6, 1, 0, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5325, 39, 8, 408, 19, '2021-01-21 19:47:48', 7, 2, 0, 1, 4, NULL, NULL);
INSERT INTO `stock` VALUES (5327, 36, 4, 204, 20, '2021-01-21 19:54:45', 2, 0, 0, 1, 4, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 10420 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_trans
-- ----------------------------

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
-- Records of tile_track
-- ----------------------------

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
  `rfids` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '轨道所有地标（用 # 隔开）',
  `split_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道分段坐标点',
  `limit_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道下砖极限坐标点',
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '顺序',
  `recent_goodid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '最近上砖/下砖规格',
  `recent_tileid` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '最近上/下砖机ID',
  `alert_status` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '故障状态',
  `alert_carrier` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '故障小车',
  `alert_trans` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '故障任务',
  `early_full` bit(1) NULL DEFAULT NULL COMMENT '提前满砖',
  `full_time` datetime(0) NULL DEFAULT NULL COMMENT '满砖时间',
  `same_side_inout` bit(1) NULL DEFAULT NULL COMMENT '是否同侧出入库',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 75 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of track
-- ----------------------------
INSERT INTO `track` VALUES (1, '01_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 101, 100, 0, 0, 0, NULL, 101, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (2, '02_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 102, 100, 0, 0, 0, NULL, 102, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (3, '03_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 103, 100, 0, 0, 0, NULL, 103, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (4, '04_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 104, 100, 0, 0, 0, NULL, 104, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (5, '05_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 105, 100, 0, 0, 0, NULL, 105, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (6, '06_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 106, 100, 0, 0, 0, NULL, 106, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 10, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (7, '07_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 107, 100, 0, 0, 0, NULL, 107, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 12, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (8, '08_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 108, 100, 0, 0, 0, NULL, 108, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 13, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (9, '09_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 109, 100, 0, 0, 0, NULL, 109, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 15, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (10, '10_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 110, 100, 0, 0, 0, NULL, 110, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 16, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (11, '11_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 111, 100, 0, 0, 0, NULL, 111, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 18, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (12, '12_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 112, 100, 0, 0, 0, NULL, 112, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 19, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (13, '13_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 113, 100, 0, 0, 0, NULL, 113, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 21, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (14, '14_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 114, 100, 0, 0, 0, NULL, 114, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 22, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (15, '15_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 115, 100, 0, 0, 0, NULL, 115, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 24, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (16, '16_下砖轨', 1, 1, 1, 0, 0, 0, 0, 0, 116, 100, 0, 0, 0, NULL, 116, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 25, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (17, '01_上砖轨', 1, 0, 1, 0, 0, 0, 0, 601, 0, 100, 0, 0, 0, NULL, 601, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (18, '02_上砖轨', 1, 0, 1, 0, 0, 0, 0, 602, 0, 100, 0, 0, 0, NULL, 602, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (19, '01_储砖轨', 1, 4, 2, 0, 700, 350, 350, 201, 501, 100, 0, 0, 20, NULL, 30100, 30110, NULL, NULL, NULL, NULL, '30100#30101#30102#30103#30104#30105#30106#30107#30108#30109#30110#', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (20, '02_储砖轨', 1, 4, 2, 0, 700, 350, 350, 202, 502, 100, 0, 19, 21, NULL, 202, 502, NULL, NULL, NULL, NULL, '30200#30201#30202#30203#30204#30205#30206#30207#30208#30209#30210#', NULL, NULL, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (21, '03_储砖轨', 1, 4, 2, 0, 700, 350, 350, 203, 503, 100, 0, 20, 22, NULL, 203, 503, NULL, NULL, NULL, NULL, '30300#30301#30302#30303#30304#30305#30306#30307#30308#30309#30310#', NULL, NULL, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (22, '04_储砖轨', 1, 4, 2, 0, 700, 350, 350, 204, 504, 100, 0, 21, 23, NULL, 204, 504, NULL, NULL, NULL, NULL, '30400#30401#30402#30403#30404#30405#30406#30407#30408#30409#30410#', NULL, NULL, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (23, '05_储砖轨', 1, 4, 1, 0, 700, 350, 350, 205, 505, 100, 0, 22, 24, NULL, 205, 505, NULL, NULL, NULL, NULL, '30500#30501#30502#30503#30504#30505#30506#30507#30508#30509#30510#', NULL, NULL, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (24, '06_储砖轨', 1, 4, 2, 0, 700, 350, 350, 206, 506, 100, 0, 23, 25, NULL, 206, 506, NULL, NULL, NULL, NULL, '30600#30601#30602#30603#30604#30605#30606#30607#30608#30609#30610#', NULL, NULL, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (25, '07_储砖轨', 1, 4, 2, 0, 700, 350, 350, 207, 507, 100, 0, 24, 26, NULL, 207, 507, NULL, NULL, NULL, NULL, '30700#30701#30702#30703#30704#30705#30706#30707#30708#30709#30710#', NULL, NULL, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (26, '08_储砖轨', 1, 4, 2, 0, 700, 350, 350, 208, 508, 100, 0, 25, 27, NULL, 208, 508, NULL, NULL, NULL, NULL, '30800#30801#30802#30803#30804#30805#30806#30807#30808#30809#30810#', NULL, NULL, 8, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (27, '09_储砖轨', 1, 4, 2, 0, 700, 350, 350, 209, 509, 100, 0, 26, 28, NULL, 209, 509, NULL, NULL, NULL, NULL, '30900#30901#30902#30903#30904#30905#30906#30907#30908#30909#30910#', NULL, NULL, 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (28, '10_储砖轨', 1, 4, 2, 0, 700, 350, 350, 210, 510, 100, 0, 27, 29, NULL, 210, 510, NULL, NULL, NULL, NULL, '31000#31001#31002#31003#31004#31005#31006#31007#31008#31009#31010#', NULL, NULL, 10, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (29, '11_储砖轨', 1, 4, 1, 0, 700, 350, 350, 211, 511, 100, 0, 28, 30, NULL, 211, 511, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 11, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (30, '12_储砖轨', 1, 4, 0, 0, 700, 350, 350, 212, 512, 100, 0, 29, 31, NULL, 212, 512, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 12, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (31, '13_储砖轨', 1, 4, 2, 0, 700, 350, 350, 213, 513, 100, 0, 30, 32, NULL, 213, 513, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 13, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (32, '14_储砖轨', 1, 4, 2, 0, 700, 350, 350, 214, 514, 100, 0, 31, 33, NULL, 214, 514, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 14, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (33, '15_储砖轨', 1, 4, 2, 0, 700, 350, 350, 215, 515, 100, 0, 32, 34, NULL, 215, 515, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 15, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (34, '16_储砖轨', 1, 4, 0, 0, 700, 350, 350, 216, 516, 100, 0, 33, 35, NULL, 216, 516, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 16, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (35, '17_储砖轨', 1, 4, 2, 0, 700, 350, 350, 217, 517, 100, 0, 34, 36, NULL, 217, 517, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 17, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (36, '18_储砖轨', 1, 4, 2, 0, 700, 350, 350, 218, 518, 100, 0, 35, 37, NULL, 218, 518, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 18, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (37, '19_储砖轨', 1, 4, 2, 0, 700, 350, 350, 219, 519, 100, 0, 36, 38, NULL, 219, 519, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 19, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (38, '20_储砖轨', 1, 4, 2, 0, 700, 350, 350, 220, 520, 100, 0, 37, 39, NULL, 220, 520, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 20, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (39, '21_储砖轨', 1, 4, 2, 0, 700, 350, 350, 221, 521, 100, 0, 38, 40, NULL, 221, 521, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 21, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (40, '22_储砖轨', 1, 4, 2, 0, 700, 350, 350, 222, 522, 100, 0, 39, 41, NULL, 222, 522, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 22, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (41, '23_储砖轨', 1, 4, 0, 0, 700, 350, 350, 223, 523, 100, 0, 40, 42, NULL, 223, 523, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 23, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (42, '24_储砖轨', 1, 4, 2, 0, 700, 350, 350, 224, 524, 100, 0, 41, 43, NULL, 224, 524, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 24, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (43, '25_储砖轨', 1, 4, 0, 0, 700, 350, 350, 225, 525, 100, 0, 42, 44, NULL, 225, 525, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 25, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (44, '26_储砖轨', 1, 4, 1, 0, 700, 350, 350, 226, 526, 100, 0, 43, 0, NULL, 226, 526, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 26, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (45, 'B1_摆渡轨', 1, 5, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 701, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (46, 'B2_摆渡轨', 1, 5, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 702, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (47, 'B5_摆渡轨', 1, 6, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 741, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (48, 'B6_摆渡轨', 1, 6, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 742, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (49, '03_上砖轨', 1, 0, 1, 0, 0, 0, 0, 603, 0, 100, 0, 0, 0, NULL, 603, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 14, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (50, '04_上砖轨', 1, 0, 1, 0, 0, 0, 0, 604, 0, 100, 0, 0, 0, NULL, 604, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 17, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 3095 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of track_log
-- ----------------------------

-- ----------------------------
-- Table structure for warning
-- ----------------------------
DROP TABLE IF EXISTS `warning`;
CREATE TABLE `warning`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `area_id` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `type` tinyint(3) UNSIGNED NULL DEFAULT NULL,
  `resolve` bit(1) NULL DEFAULT NULL COMMENT '是否解决',
  `dev_id` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `track_id` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '轨道ID',
  `trans_id` int(10) UNSIGNED NOT NULL,
  `content` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `createtime` datetime(0) NULL DEFAULT NULL COMMENT '报警时间',
  `resolvetime` datetime(0) NULL DEFAULT NULL COMMENT '解决时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `w_createtime_idx`(`createtime`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6848 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of warning
-- ----------------------------
INSERT INTO `warning` VALUES (6833, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-12 09:07:21', '2021-01-12 09:07:43');
INSERT INTO `warning` VALUES (6834, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-12 09:15:01', '2021-01-12 09:18:33');
INSERT INTO `warning` VALUES (6835, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-12 09:22:10', '2021-01-12 09:26:03');
INSERT INTO `warning` VALUES (6836, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-12 09:41:23', '2021-01-12 09:47:53');
INSERT INTO `warning` VALUES (6837, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-12 09:52:18', '2021-01-12 09:52:28');
INSERT INTO `warning` VALUES (6838, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-12 09:52:38', '2021-01-12 09:54:27');
INSERT INTO `warning` VALUES (6839, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-12 10:41:09', '2021-01-12 10:43:31');
INSERT INTO `warning` VALUES (6840, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-12 13:59:48', '2021-01-15 17:18:58');
INSERT INTO `warning` VALUES (6841, 0, 0, b'1', 15, 0, 0, 'C01: 设备离线', '2021-01-15 13:58:05', '2021-01-15 13:58:06');
INSERT INTO `warning` VALUES (6842, 0, 0, b'1', 1, 0, 0, 'A01: 设备离线', '2021-01-15 17:19:08', '2021-01-19 14:57:16');
INSERT INTO `warning` VALUES (6843, 0, 0, b'1', 12, 0, 0, 'B02: 设备离线', '2021-01-16 08:25:34', '2021-01-16 08:25:38');
INSERT INTO `warning` VALUES (6844, 0, 0, b'1', 15, 0, 0, 'C01: 设备离线', '2021-01-19 14:56:22', '2021-01-19 14:57:07');
INSERT INTO `warning` VALUES (6845, 0, 0, b'1', 15, 0, 0, 'C01: 设备离线', '2021-01-19 17:00:44', '2021-01-19 17:25:29');
INSERT INTO `warning` VALUES (6846, 0, 0, b'1', 15, 0, 0, 'C01: 设备离线', '2021-01-19 17:26:57', '2021-01-19 17:48:52');
INSERT INTO `warning` VALUES (6847, 0, 0, b'1', 15, 0, 0, 'C01: 设备离线', '2021-01-20 08:18:24', '2021-01-20 08:18:42');

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
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `active_dev` AS select `t`.`id` AS `id`,`t`.`name` AS `name`,`t`.`ip` AS `ip`,`t`.`port` AS `port`,`t`.`type` AS `type`,`t`.`type2` AS `type2`,`t`.`enable` AS `enable`,`t`.`att1` AS `att1`,`t`.`att2` AS `att2`,`t`.`memo` AS `memo` from `device` `t` where (`t`.`enable` = 1);

-- ----------------------------
-- View structure for stock_sum
-- ----------------------------
DROP VIEW IF EXISTS `stock_sum`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `stock_sum` AS select `t`.`goods_id` AS `goods_id`,`t`.`track_id` AS `track_id`,min(`t`.`produce_time`) AS `produce_time`,count(`t`.`id`) AS `count`,sum(`t`.`pieces`) AS `pieces`,sum(`t`.`stack`) AS `stack`,`t`.`area` AS `area`,`t`.`track_type` AS `track_type` from `stock` `t` where (`t`.`track_type` in (2,3,4)) group by `t`.`track_id`,`t`.`goods_id` order by `t`.`area`,`t`.`goods_id`,`produce_time`,`t`.`track_id`;

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
EVERY '1' DAY STARTS '2020-10-02 01:00:00'
DO CALL DELETE_DATA()
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
