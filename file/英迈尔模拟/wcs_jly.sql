/*
 Navicat Premium Data Transfer

 Source Server         : mysql
 Source Server Type    : MySQL
 Source Server Version : 80016
 Source Host           : localhost:3306
 Source Schema         : wcs_jly

 Target Server Type    : MySQL
 Target Server Version : 80016
 File Encoding         : 65001

 Date: 24/03/2021 14:42:29
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
INSERT INTO `area` VALUES (1, '窑后#', b'1', b'1', '窑后', 3, 0, 21);
INSERT INTO `area` VALUES (2, 'A#', b'1', b'1', 'A线', 2, 0, 10);

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
) ENGINE = InnoDB AUTO_INCREMENT = 32 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域-设备-关系表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_device
-- ----------------------------
INSERT INTO `area_device` VALUES (1, 1, 1, 1);
INSERT INTO `area_device` VALUES (2, 1, 2, 1);
INSERT INTO `area_device` VALUES (3, 1, 3, 1);
INSERT INTO `area_device` VALUES (4, 1, 4, 1);
INSERT INTO `area_device` VALUES (5, 1, 5, 1);
INSERT INTO `area_device` VALUES (6, 1, 6, 1);
INSERT INTO `area_device` VALUES (7, 1, 7, 0);
INSERT INTO `area_device` VALUES (8, 1, 8, 0);
INSERT INTO `area_device` VALUES (9, 1, 9, 0);
INSERT INTO `area_device` VALUES (10, 1, 10, 3);
INSERT INTO `area_device` VALUES (11, 1, 11, 3);
INSERT INTO `area_device` VALUES (12, 1, 12, 2);
INSERT INTO `area_device` VALUES (13, 1, 13, 2);
INSERT INTO `area_device` VALUES (14, 1, 14, 4);
INSERT INTO `area_device` VALUES (15, 1, 15, 4);
INSERT INTO `area_device` VALUES (16, 1, 16, 4);
INSERT INTO `area_device` VALUES (17, 1, 17, 4);
INSERT INTO `area_device` VALUES (18, 1, 18, 4);
INSERT INTO `area_device` VALUES (19, 1, 19, 4);
INSERT INTO `area_device` VALUES (20, 1, 20, 4);
INSERT INTO `area_device` VALUES (21, 1, 21, 4);
INSERT INTO `area_device` VALUES (22, 1, 22, 4);
INSERT INTO `area_device` VALUES (23, 1, 23, 4);
INSERT INTO `area_device` VALUES (24, 2, 24, 1);
INSERT INTO `area_device` VALUES (25, 2, 25, 1);
INSERT INTO `area_device` VALUES (26, 2, 26, 1);
INSERT INTO `area_device` VALUES (27, 2, 27, 0);
INSERT INTO `area_device` VALUES (28, 2, 28, 3);
INSERT INTO `area_device` VALUES (29, 2, 29, 2);
INSERT INTO `area_device` VALUES (30, 2, 30, 4);
INSERT INTO `area_device` VALUES (31, 2, 31, 4);
INSERT INTO `area_device` VALUES (32, 2, 32, 4);

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
) ENGINE = InnoDB AUTO_INCREMENT = 1366 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域-设备-轨道-关系表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of area_device_track
-- ----------------------------
INSERT INTO `area_device_track` VALUES (1, 1, 10, 19, 1);
INSERT INTO `area_device_track` VALUES (2, 1, 10, 20, 2);
INSERT INTO `area_device_track` VALUES (3, 1, 10, 21, 3);
INSERT INTO `area_device_track` VALUES (4, 1, 10, 22, 4);
INSERT INTO `area_device_track` VALUES (5, 1, 10, 23, 5);
INSERT INTO `area_device_track` VALUES (6, 1, 10, 24, 6);
INSERT INTO `area_device_track` VALUES (7, 1, 10, 25, 7);
INSERT INTO `area_device_track` VALUES (8, 1, 10, 26, 8);
INSERT INTO `area_device_track` VALUES (9, 1, 10, 27, 9);
INSERT INTO `area_device_track` VALUES (10, 1, 10, 28, 10);
INSERT INTO `area_device_track` VALUES (11, 1, 10, 1, 27);
INSERT INTO `area_device_track` VALUES (12, 1, 10, 2, 28);
INSERT INTO `area_device_track` VALUES (13, 1, 10, 3, 29);
INSERT INTO `area_device_track` VALUES (14, 1, 10, 4, 30);
INSERT INTO `area_device_track` VALUES (17, 1, 11, 31, 3);
INSERT INTO `area_device_track` VALUES (18, 1, 11, 32, 4);
INSERT INTO `area_device_track` VALUES (19, 1, 11, 33, 5);
INSERT INTO `area_device_track` VALUES (20, 1, 11, 34, 6);
INSERT INTO `area_device_track` VALUES (21, 1, 11, 35, 7);
INSERT INTO `area_device_track` VALUES (22, 1, 11, 36, 8);
INSERT INTO `area_device_track` VALUES (23, 1, 11, 37, 9);
INSERT INTO `area_device_track` VALUES (24, 1, 11, 38, 10);
INSERT INTO `area_device_track` VALUES (25, 1, 11, 39, 11);
INSERT INTO `area_device_track` VALUES (26, 1, 11, 40, 12);
INSERT INTO `area_device_track` VALUES (27, 1, 11, 41, 13);
INSERT INTO `area_device_track` VALUES (28, 1, 11, 42, 14);
INSERT INTO `area_device_track` VALUES (29, 1, 11, 43, 15);
INSERT INTO `area_device_track` VALUES (30, 1, 11, 44, 16);
INSERT INTO `area_device_track` VALUES (31, 1, 11, 45, 17);
INSERT INTO `area_device_track` VALUES (32, 1, 11, 46, 18);
INSERT INTO `area_device_track` VALUES (33, 1, 11, 7, 29);
INSERT INTO `area_device_track` VALUES (34, 1, 11, 8, 31);
INSERT INTO `area_device_track` VALUES (37, 1, 11, 11, 35);
INSERT INTO `area_device_track` VALUES (38, 1, 11, 12, 36);
INSERT INTO `area_device_track` VALUES (43, 1, 12, 51, 5);
INSERT INTO `area_device_track` VALUES (44, 1, 12, 52, 6);
INSERT INTO `area_device_track` VALUES (45, 1, 12, 53, 7);
INSERT INTO `area_device_track` VALUES (46, 1, 12, 54, 8);
INSERT INTO `area_device_track` VALUES (47, 1, 12, 55, 9);
INSERT INTO `area_device_track` VALUES (48, 1, 12, 56, 10);
INSERT INTO `area_device_track` VALUES (49, 1, 12, 13, 27);
INSERT INTO `area_device_track` VALUES (50, 1, 12, 14, 28);
INSERT INTO `area_device_track` VALUES (53, 1, 13, 59, 3);
INSERT INTO `area_device_track` VALUES (54, 1, 13, 60, 4);
INSERT INTO `area_device_track` VALUES (55, 1, 13, 61, 5);
INSERT INTO `area_device_track` VALUES (56, 1, 13, 62, 6);
INSERT INTO `area_device_track` VALUES (57, 1, 13, 63, 7);
INSERT INTO `area_device_track` VALUES (58, 1, 13, 64, 8);
INSERT INTO `area_device_track` VALUES (59, 1, 13, 65, 9);
INSERT INTO `area_device_track` VALUES (60, 1, 13, 66, 10);
INSERT INTO `area_device_track` VALUES (61, 1, 13, 67, 11);
INSERT INTO `area_device_track` VALUES (62, 1, 13, 68, 12);
INSERT INTO `area_device_track` VALUES (63, 1, 13, 69, 13);
INSERT INTO `area_device_track` VALUES (64, 1, 13, 70, 14);
INSERT INTO `area_device_track` VALUES (65, 1, 13, 71, 15);
INSERT INTO `area_device_track` VALUES (66, 1, 13, 72, 16);
INSERT INTO `area_device_track` VALUES (67, 1, 13, 73, 17);
INSERT INTO `area_device_track` VALUES (68, 1, 13, 74, 18);
INSERT INTO `area_device_track` VALUES (69, 1, 13, 15, 29);
INSERT INTO `area_device_track` VALUES (70, 1, 13, 16, 30);
INSERT INTO `area_device_track` VALUES (71, 1, 13, 17, 31);
INSERT INTO `area_device_track` VALUES (72, 1, 13, 18, 32);
INSERT INTO `area_device_track` VALUES (73, 1, 1, 19, 1);
INSERT INTO `area_device_track` VALUES (74, 1, 1, 20, 2);
INSERT INTO `area_device_track` VALUES (75, 1, 1, 21, 3);
INSERT INTO `area_device_track` VALUES (76, 1, 1, 22, 4);
INSERT INTO `area_device_track` VALUES (77, 1, 1, 23, 5);
INSERT INTO `area_device_track` VALUES (78, 1, 1, 24, 6);
INSERT INTO `area_device_track` VALUES (79, 1, 1, 25, 7);
INSERT INTO `area_device_track` VALUES (80, 1, 1, 26, 8);
INSERT INTO `area_device_track` VALUES (81, 1, 1, 27, 9);
INSERT INTO `area_device_track` VALUES (82, 1, 1, 28, 10);
INSERT INTO `area_device_track` VALUES (83, 1, 2, 19, 1);
INSERT INTO `area_device_track` VALUES (84, 1, 2, 20, 2);
INSERT INTO `area_device_track` VALUES (85, 1, 2, 21, 3);
INSERT INTO `area_device_track` VALUES (86, 1, 2, 22, 4);
INSERT INTO `area_device_track` VALUES (87, 1, 2, 23, 5);
INSERT INTO `area_device_track` VALUES (88, 1, 2, 24, 6);
INSERT INTO `area_device_track` VALUES (89, 1, 2, 25, 7);
INSERT INTO `area_device_track` VALUES (90, 1, 2, 26, 8);
INSERT INTO `area_device_track` VALUES (91, 1, 2, 27, 9);
INSERT INTO `area_device_track` VALUES (92, 1, 2, 28, 10);
INSERT INTO `area_device_track` VALUES (95, 1, 4, 31, 3);
INSERT INTO `area_device_track` VALUES (96, 1, 4, 32, 4);
INSERT INTO `area_device_track` VALUES (97, 1, 4, 33, 5);
INSERT INTO `area_device_track` VALUES (98, 1, 4, 34, 6);
INSERT INTO `area_device_track` VALUES (99, 1, 4, 35, 7);
INSERT INTO `area_device_track` VALUES (100, 1, 4, 36, 8);
INSERT INTO `area_device_track` VALUES (101, 1, 4, 37, 9);
INSERT INTO `area_device_track` VALUES (102, 1, 4, 38, 10);
INSERT INTO `area_device_track` VALUES (103, 1, 6, 39, 1);
INSERT INTO `area_device_track` VALUES (104, 1, 6, 40, 2);
INSERT INTO `area_device_track` VALUES (105, 1, 6, 41, 3);
INSERT INTO `area_device_track` VALUES (106, 1, 6, 42, 4);
INSERT INTO `area_device_track` VALUES (107, 1, 6, 43, 5);
INSERT INTO `area_device_track` VALUES (108, 1, 6, 44, 6);
INSERT INTO `area_device_track` VALUES (109, 1, 6, 45, 7);
INSERT INTO `area_device_track` VALUES (110, 1, 6, 46, 8);
INSERT INTO `area_device_track` VALUES (111, 1, 7, 47, 1);
INSERT INTO `area_device_track` VALUES (112, 1, 7, 48, 2);
INSERT INTO `area_device_track` VALUES (113, 1, 7, 49, 3);
INSERT INTO `area_device_track` VALUES (114, 1, 7, 50, 4);
INSERT INTO `area_device_track` VALUES (115, 1, 7, 51, 5);
INSERT INTO `area_device_track` VALUES (116, 1, 7, 52, 6);
INSERT INTO `area_device_track` VALUES (117, 1, 7, 53, 7);
INSERT INTO `area_device_track` VALUES (118, 1, 7, 54, 8);
INSERT INTO `area_device_track` VALUES (119, 1, 7, 55, 9);
INSERT INTO `area_device_track` VALUES (120, 1, 7, 56, 10);
INSERT INTO `area_device_track` VALUES (123, 1, 8, 59, 3);
INSERT INTO `area_device_track` VALUES (124, 1, 8, 60, 4);
INSERT INTO `area_device_track` VALUES (125, 1, 8, 61, 5);
INSERT INTO `area_device_track` VALUES (126, 1, 8, 62, 6);
INSERT INTO `area_device_track` VALUES (127, 1, 8, 63, 7);
INSERT INTO `area_device_track` VALUES (128, 1, 8, 64, 8);
INSERT INTO `area_device_track` VALUES (129, 1, 8, 65, 9);
INSERT INTO `area_device_track` VALUES (130, 1, 8, 66, 10);
INSERT INTO `area_device_track` VALUES (131, 1, 9, 67, 1);
INSERT INTO `area_device_track` VALUES (132, 1, 9, 68, 2);
INSERT INTO `area_device_track` VALUES (133, 1, 9, 69, 3);
INSERT INTO `area_device_track` VALUES (134, 1, 9, 70, 4);
INSERT INTO `area_device_track` VALUES (135, 1, 9, 71, 5);
INSERT INTO `area_device_track` VALUES (136, 1, 9, 72, 6);
INSERT INTO `area_device_track` VALUES (137, 1, 9, 73, 7);
INSERT INTO `area_device_track` VALUES (138, 1, 9, 74, 8);
INSERT INTO `area_device_track` VALUES (139, 2, 28, 87, 1);
INSERT INTO `area_device_track` VALUES (140, 2, 28, 88, 2);
INSERT INTO `area_device_track` VALUES (141, 2, 28, 89, 3);
INSERT INTO `area_device_track` VALUES (142, 2, 28, 90, 4);
INSERT INTO `area_device_track` VALUES (143, 2, 28, 91, 5);
INSERT INTO `area_device_track` VALUES (144, 2, 28, 92, 6);
INSERT INTO `area_device_track` VALUES (145, 2, 28, 93, 7);
INSERT INTO `area_device_track` VALUES (146, 2, 28, 94, 8);
INSERT INTO `area_device_track` VALUES (147, 2, 28, 95, 9);
INSERT INTO `area_device_track` VALUES (148, 2, 28, 96, 10);
INSERT INTO `area_device_track` VALUES (149, 2, 28, 97, 11);
INSERT INTO `area_device_track` VALUES (150, 2, 28, 98, 12);
INSERT INTO `area_device_track` VALUES (151, 2, 28, 99, 13);
INSERT INTO `area_device_track` VALUES (152, 2, 28, 100, 14);
INSERT INTO `area_device_track` VALUES (153, 2, 28, 101, 15);
INSERT INTO `area_device_track` VALUES (154, 2, 28, 102, 16);
INSERT INTO `area_device_track` VALUES (155, 2, 28, 103, 17);
INSERT INTO `area_device_track` VALUES (156, 2, 28, 104, 18);
INSERT INTO `area_device_track` VALUES (157, 2, 28, 105, 19);
INSERT INTO `area_device_track` VALUES (158, 2, 28, 106, 20);
INSERT INTO `area_device_track` VALUES (159, 2, 28, 107, 21);
INSERT INTO `area_device_track` VALUES (160, 2, 28, 108, 22);
INSERT INTO `area_device_track` VALUES (161, 2, 28, 79, 23);
INSERT INTO `area_device_track` VALUES (162, 2, 28, 80, 24);
INSERT INTO `area_device_track` VALUES (163, 2, 28, 81, 25);
INSERT INTO `area_device_track` VALUES (164, 2, 28, 82, 26);
INSERT INTO `area_device_track` VALUES (165, 2, 28, 83, 27);
INSERT INTO `area_device_track` VALUES (166, 2, 28, 84, 28);
INSERT INTO `area_device_track` VALUES (167, 2, 29, 87, 1);
INSERT INTO `area_device_track` VALUES (168, 2, 29, 88, 2);
INSERT INTO `area_device_track` VALUES (169, 2, 29, 89, 3);
INSERT INTO `area_device_track` VALUES (170, 2, 29, 90, 4);
INSERT INTO `area_device_track` VALUES (171, 2, 29, 91, 5);
INSERT INTO `area_device_track` VALUES (172, 2, 29, 92, 6);
INSERT INTO `area_device_track` VALUES (173, 2, 29, 93, 7);
INSERT INTO `area_device_track` VALUES (174, 2, 29, 94, 8);
INSERT INTO `area_device_track` VALUES (175, 2, 29, 95, 9);
INSERT INTO `area_device_track` VALUES (176, 2, 29, 96, 10);
INSERT INTO `area_device_track` VALUES (177, 2, 29, 97, 11);
INSERT INTO `area_device_track` VALUES (178, 2, 29, 98, 12);
INSERT INTO `area_device_track` VALUES (179, 2, 29, 99, 13);
INSERT INTO `area_device_track` VALUES (180, 2, 29, 100, 14);
INSERT INTO `area_device_track` VALUES (181, 2, 29, 101, 15);
INSERT INTO `area_device_track` VALUES (182, 2, 29, 102, 16);
INSERT INTO `area_device_track` VALUES (183, 2, 29, 103, 17);
INSERT INTO `area_device_track` VALUES (184, 2, 29, 104, 18);
INSERT INTO `area_device_track` VALUES (185, 2, 29, 105, 19);
INSERT INTO `area_device_track` VALUES (186, 2, 29, 106, 20);
INSERT INTO `area_device_track` VALUES (187, 2, 29, 107, 21);
INSERT INTO `area_device_track` VALUES (188, 2, 29, 108, 22);
INSERT INTO `area_device_track` VALUES (189, 2, 29, 85, 23);
INSERT INTO `area_device_track` VALUES (190, 2, 29, 86, 24);
INSERT INTO `area_device_track` VALUES (191, 2, 24, 87, 1);
INSERT INTO `area_device_track` VALUES (192, 2, 24, 88, 2);
INSERT INTO `area_device_track` VALUES (193, 2, 24, 89, 3);
INSERT INTO `area_device_track` VALUES (194, 2, 24, 90, 4);
INSERT INTO `area_device_track` VALUES (195, 2, 24, 91, 5);
INSERT INTO `area_device_track` VALUES (196, 2, 24, 92, 6);
INSERT INTO `area_device_track` VALUES (197, 2, 24, 93, 7);
INSERT INTO `area_device_track` VALUES (198, 2, 24, 94, 8);
INSERT INTO `area_device_track` VALUES (199, 2, 24, 95, 9);
INSERT INTO `area_device_track` VALUES (200, 2, 24, 96, 10);
INSERT INTO `area_device_track` VALUES (201, 2, 24, 97, 11);
INSERT INTO `area_device_track` VALUES (202, 2, 24, 98, 12);
INSERT INTO `area_device_track` VALUES (203, 2, 24, 99, 13);
INSERT INTO `area_device_track` VALUES (204, 2, 24, 100, 14);
INSERT INTO `area_device_track` VALUES (205, 2, 24, 101, 15);
INSERT INTO `area_device_track` VALUES (206, 2, 24, 102, 16);
INSERT INTO `area_device_track` VALUES (207, 2, 24, 103, 17);
INSERT INTO `area_device_track` VALUES (208, 2, 24, 104, 18);
INSERT INTO `area_device_track` VALUES (209, 2, 24, 105, 19);
INSERT INTO `area_device_track` VALUES (210, 2, 24, 106, 20);
INSERT INTO `area_device_track` VALUES (211, 2, 24, 107, 21);
INSERT INTO `area_device_track` VALUES (212, 2, 24, 108, 22);
INSERT INTO `area_device_track` VALUES (213, 2, 25, 87, 1);
INSERT INTO `area_device_track` VALUES (214, 2, 25, 88, 2);
INSERT INTO `area_device_track` VALUES (215, 2, 25, 89, 3);
INSERT INTO `area_device_track` VALUES (216, 2, 25, 90, 4);
INSERT INTO `area_device_track` VALUES (217, 2, 25, 91, 5);
INSERT INTO `area_device_track` VALUES (218, 2, 25, 92, 6);
INSERT INTO `area_device_track` VALUES (219, 2, 25, 93, 7);
INSERT INTO `area_device_track` VALUES (220, 2, 25, 94, 8);
INSERT INTO `area_device_track` VALUES (221, 2, 25, 95, 9);
INSERT INTO `area_device_track` VALUES (222, 2, 25, 96, 10);
INSERT INTO `area_device_track` VALUES (223, 2, 25, 97, 11);
INSERT INTO `area_device_track` VALUES (224, 2, 25, 98, 12);
INSERT INTO `area_device_track` VALUES (225, 2, 25, 99, 13);
INSERT INTO `area_device_track` VALUES (226, 2, 25, 100, 14);
INSERT INTO `area_device_track` VALUES (227, 2, 25, 101, 15);
INSERT INTO `area_device_track` VALUES (228, 2, 25, 102, 16);
INSERT INTO `area_device_track` VALUES (229, 2, 25, 103, 17);
INSERT INTO `area_device_track` VALUES (230, 2, 25, 104, 18);
INSERT INTO `area_device_track` VALUES (231, 2, 25, 105, 19);
INSERT INTO `area_device_track` VALUES (232, 2, 25, 106, 20);
INSERT INTO `area_device_track` VALUES (233, 2, 25, 107, 21);
INSERT INTO `area_device_track` VALUES (234, 2, 25, 108, 22);
INSERT INTO `area_device_track` VALUES (235, 2, 26, 87, 1);
INSERT INTO `area_device_track` VALUES (236, 2, 26, 88, 2);
INSERT INTO `area_device_track` VALUES (237, 2, 26, 89, 3);
INSERT INTO `area_device_track` VALUES (238, 2, 26, 90, 4);
INSERT INTO `area_device_track` VALUES (239, 2, 26, 91, 5);
INSERT INTO `area_device_track` VALUES (240, 2, 26, 92, 6);
INSERT INTO `area_device_track` VALUES (241, 2, 26, 93, 7);
INSERT INTO `area_device_track` VALUES (242, 2, 26, 94, 8);
INSERT INTO `area_device_track` VALUES (243, 2, 26, 95, 9);
INSERT INTO `area_device_track` VALUES (244, 2, 26, 96, 10);
INSERT INTO `area_device_track` VALUES (245, 2, 26, 97, 11);
INSERT INTO `area_device_track` VALUES (246, 2, 26, 98, 12);
INSERT INTO `area_device_track` VALUES (247, 2, 26, 99, 13);
INSERT INTO `area_device_track` VALUES (248, 2, 26, 100, 14);
INSERT INTO `area_device_track` VALUES (249, 2, 26, 101, 15);
INSERT INTO `area_device_track` VALUES (250, 2, 26, 102, 16);
INSERT INTO `area_device_track` VALUES (251, 2, 26, 103, 17);
INSERT INTO `area_device_track` VALUES (252, 2, 26, 104, 18);
INSERT INTO `area_device_track` VALUES (253, 2, 26, 105, 19);
INSERT INTO `area_device_track` VALUES (254, 2, 26, 106, 20);
INSERT INTO `area_device_track` VALUES (255, 2, 26, 107, 21);
INSERT INTO `area_device_track` VALUES (256, 2, 26, 108, 22);
INSERT INTO `area_device_track` VALUES (257, 2, 27, 87, 1);
INSERT INTO `area_device_track` VALUES (258, 2, 27, 88, 2);
INSERT INTO `area_device_track` VALUES (259, 2, 27, 89, 3);
INSERT INTO `area_device_track` VALUES (260, 2, 27, 90, 4);
INSERT INTO `area_device_track` VALUES (261, 2, 27, 91, 5);
INSERT INTO `area_device_track` VALUES (262, 2, 27, 92, 6);
INSERT INTO `area_device_track` VALUES (263, 2, 27, 93, 7);
INSERT INTO `area_device_track` VALUES (264, 2, 27, 94, 8);
INSERT INTO `area_device_track` VALUES (265, 2, 27, 95, 9);
INSERT INTO `area_device_track` VALUES (266, 2, 27, 96, 10);
INSERT INTO `area_device_track` VALUES (267, 2, 27, 97, 11);
INSERT INTO `area_device_track` VALUES (268, 2, 27, 98, 12);
INSERT INTO `area_device_track` VALUES (269, 2, 27, 99, 13);
INSERT INTO `area_device_track` VALUES (270, 2, 27, 100, 14);
INSERT INTO `area_device_track` VALUES (271, 2, 27, 101, 15);
INSERT INTO `area_device_track` VALUES (272, 2, 27, 102, 16);
INSERT INTO `area_device_track` VALUES (273, 2, 27, 103, 17);
INSERT INTO `area_device_track` VALUES (274, 2, 27, 104, 18);
INSERT INTO `area_device_track` VALUES (275, 2, 27, 105, 19);
INSERT INTO `area_device_track` VALUES (276, 2, 27, 106, 20);
INSERT INTO `area_device_track` VALUES (277, 2, 27, 107, 21);
INSERT INTO `area_device_track` VALUES (278, 2, 27, 108, 22);
INSERT INTO `area_device_track` VALUES (485, 1, 12, 47, 1);
INSERT INTO `area_device_track` VALUES (486, 1, 12, 48, 2);
INSERT INTO `area_device_track` VALUES (487, 1, 12, 49, 3);
INSERT INTO `area_device_track` VALUES (488, 1, 12, 50, 4);
INSERT INTO `area_device_track` VALUES (492, 1, 1, 29, 11);
INSERT INTO `area_device_track` VALUES (493, 1, 1, 30, 12);
INSERT INTO `area_device_track` VALUES (508, 1, 2, 29, 11);
INSERT INTO `area_device_track` VALUES (509, 1, 2, 30, 12);
INSERT INTO `area_device_track` VALUES (562, 1, 10, 29, 11);
INSERT INTO `area_device_track` VALUES (563, 1, 10, 30, 12);
INSERT INTO `area_device_track` VALUES (598, 1, 12, 57, 11);
INSERT INTO `area_device_track` VALUES (599, 1, 12, 58, 12);
INSERT INTO `area_device_track` VALUES (628, 1, 7, 57, 11);
INSERT INTO `area_device_track` VALUES (629, 1, 7, 58, 12);
INSERT INTO `area_device_track` VALUES (1350, 1, 3, 19, 1);
INSERT INTO `area_device_track` VALUES (1351, 1, 3, 20, 2);
INSERT INTO `area_device_track` VALUES (1352, 1, 3, 21, 3);
INSERT INTO `area_device_track` VALUES (1353, 1, 3, 22, 4);
INSERT INTO `area_device_track` VALUES (1354, 1, 3, 23, 5);
INSERT INTO `area_device_track` VALUES (1355, 1, 3, 24, 6);
INSERT INTO `area_device_track` VALUES (1356, 1, 3, 25, 7);
INSERT INTO `area_device_track` VALUES (1357, 1, 3, 26, 8);
INSERT INTO `area_device_track` VALUES (1358, 1, 3, 27, 9);
INSERT INTO `area_device_track` VALUES (1359, 1, 3, 28, 10);
INSERT INTO `area_device_track` VALUES (1360, 1, 3, 29, 11);
INSERT INTO `area_device_track` VALUES (1361, 1, 3, 30, 12);
INSERT INTO `area_device_track` VALUES (1365, 1, 10, 5, 29);
INSERT INTO `area_device_track` VALUES (1366, 1, 10, 6, 30);

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
) ENGINE = InnoDB AUTO_INCREMENT = 110 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '区域-轨道-关系表' ROW_FORMAT = Dynamic;

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
INSERT INTO `area_track` VALUES (13, 1, 13, 0);
INSERT INTO `area_track` VALUES (14, 1, 14, 0);
INSERT INTO `area_track` VALUES (15, 1, 15, 0);
INSERT INTO `area_track` VALUES (16, 1, 16, 0);
INSERT INTO `area_track` VALUES (17, 1, 17, 0);
INSERT INTO `area_track` VALUES (18, 1, 18, 0);
INSERT INTO `area_track` VALUES (19, 1, 19, 2);
INSERT INTO `area_track` VALUES (20, 1, 20, 2);
INSERT INTO `area_track` VALUES (21, 1, 21, 2);
INSERT INTO `area_track` VALUES (22, 1, 22, 2);
INSERT INTO `area_track` VALUES (23, 1, 23, 2);
INSERT INTO `area_track` VALUES (24, 1, 24, 2);
INSERT INTO `area_track` VALUES (25, 1, 25, 2);
INSERT INTO `area_track` VALUES (26, 1, 26, 2);
INSERT INTO `area_track` VALUES (27, 1, 27, 2);
INSERT INTO `area_track` VALUES (28, 1, 28, 2);
INSERT INTO `area_track` VALUES (29, 1, 29, 2);
INSERT INTO `area_track` VALUES (30, 1, 30, 2);
INSERT INTO `area_track` VALUES (31, 1, 31, 2);
INSERT INTO `area_track` VALUES (32, 1, 32, 2);
INSERT INTO `area_track` VALUES (33, 1, 33, 2);
INSERT INTO `area_track` VALUES (34, 1, 34, 2);
INSERT INTO `area_track` VALUES (35, 1, 35, 2);
INSERT INTO `area_track` VALUES (36, 1, 36, 2);
INSERT INTO `area_track` VALUES (37, 1, 37, 2);
INSERT INTO `area_track` VALUES (38, 1, 38, 2);
INSERT INTO `area_track` VALUES (39, 1, 39, 2);
INSERT INTO `area_track` VALUES (40, 1, 40, 2);
INSERT INTO `area_track` VALUES (41, 1, 41, 2);
INSERT INTO `area_track` VALUES (42, 1, 42, 2);
INSERT INTO `area_track` VALUES (43, 1, 43, 2);
INSERT INTO `area_track` VALUES (44, 1, 44, 2);
INSERT INTO `area_track` VALUES (45, 1, 45, 2);
INSERT INTO `area_track` VALUES (46, 1, 46, 2);
INSERT INTO `area_track` VALUES (47, 1, 47, 3);
INSERT INTO `area_track` VALUES (48, 1, 48, 3);
INSERT INTO `area_track` VALUES (49, 1, 49, 3);
INSERT INTO `area_track` VALUES (50, 1, 50, 3);
INSERT INTO `area_track` VALUES (51, 1, 51, 3);
INSERT INTO `area_track` VALUES (52, 1, 52, 3);
INSERT INTO `area_track` VALUES (53, 1, 53, 3);
INSERT INTO `area_track` VALUES (54, 1, 54, 3);
INSERT INTO `area_track` VALUES (55, 1, 55, 3);
INSERT INTO `area_track` VALUES (56, 1, 56, 3);
INSERT INTO `area_track` VALUES (57, 1, 57, 3);
INSERT INTO `area_track` VALUES (58, 1, 58, 3);
INSERT INTO `area_track` VALUES (59, 1, 59, 3);
INSERT INTO `area_track` VALUES (60, 1, 60, 3);
INSERT INTO `area_track` VALUES (61, 1, 61, 3);
INSERT INTO `area_track` VALUES (62, 1, 62, 3);
INSERT INTO `area_track` VALUES (63, 1, 63, 3);
INSERT INTO `area_track` VALUES (64, 1, 64, 3);
INSERT INTO `area_track` VALUES (65, 1, 65, 3);
INSERT INTO `area_track` VALUES (66, 1, 66, 3);
INSERT INTO `area_track` VALUES (67, 1, 67, 3);
INSERT INTO `area_track` VALUES (68, 1, 68, 3);
INSERT INTO `area_track` VALUES (69, 1, 69, 3);
INSERT INTO `area_track` VALUES (70, 1, 70, 3);
INSERT INTO `area_track` VALUES (71, 1, 71, 3);
INSERT INTO `area_track` VALUES (72, 1, 72, 3);
INSERT INTO `area_track` VALUES (73, 1, 73, 3);
INSERT INTO `area_track` VALUES (74, 1, 74, 3);
INSERT INTO `area_track` VALUES (75, 1, 75, 5);
INSERT INTO `area_track` VALUES (76, 1, 76, 5);
INSERT INTO `area_track` VALUES (77, 1, 77, 6);
INSERT INTO `area_track` VALUES (78, 1, 78, 6);
INSERT INTO `area_track` VALUES (79, 2, 79, 1);
INSERT INTO `area_track` VALUES (80, 2, 80, 1);
INSERT INTO `area_track` VALUES (81, 2, 81, 1);
INSERT INTO `area_track` VALUES (82, 2, 82, 1);
INSERT INTO `area_track` VALUES (83, 2, 83, 1);
INSERT INTO `area_track` VALUES (84, 2, 84, 1);
INSERT INTO `area_track` VALUES (85, 2, 85, 0);
INSERT INTO `area_track` VALUES (86, 2, 86, 0);
INSERT INTO `area_track` VALUES (87, 2, 87, 4);
INSERT INTO `area_track` VALUES (88, 2, 88, 4);
INSERT INTO `area_track` VALUES (89, 2, 89, 4);
INSERT INTO `area_track` VALUES (90, 2, 90, 4);
INSERT INTO `area_track` VALUES (91, 2, 91, 4);
INSERT INTO `area_track` VALUES (92, 2, 92, 4);
INSERT INTO `area_track` VALUES (93, 2, 93, 4);
INSERT INTO `area_track` VALUES (94, 2, 94, 4);
INSERT INTO `area_track` VALUES (95, 2, 95, 4);
INSERT INTO `area_track` VALUES (96, 2, 96, 4);
INSERT INTO `area_track` VALUES (97, 2, 97, 4);
INSERT INTO `area_track` VALUES (98, 2, 98, 4);
INSERT INTO `area_track` VALUES (99, 2, 99, 4);
INSERT INTO `area_track` VALUES (100, 2, 100, 4);
INSERT INTO `area_track` VALUES (101, 2, 101, 4);
INSERT INTO `area_track` VALUES (102, 2, 102, 4);
INSERT INTO `area_track` VALUES (103, 2, 103, 4);
INSERT INTO `area_track` VALUES (104, 2, 104, 4);
INSERT INTO `area_track` VALUES (105, 2, 105, 4);
INSERT INTO `area_track` VALUES (106, 2, 106, 4);
INSERT INTO `area_track` VALUES (107, 2, 107, 4);
INSERT INTO `area_track` VALUES (108, 2, 108, 4);
INSERT INTO `area_track` VALUES (109, 2, 109, 5);
INSERT INTO `area_track` VALUES (110, 2, 110, 6);

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
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '运输车复位坐标表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of carrier_pos
-- ----------------------------
INSERT INTO `carrier_pos` VALUES (1, 1, 20150, 1000);
INSERT INTO `carrier_pos` VALUES (2, 1, 40150, 10652);
INSERT INTO `carrier_pos` VALUES (3, 1, 20250, 1000);
INSERT INTO `carrier_pos` VALUES (4, 1, 40250, 10652);
INSERT INTO `carrier_pos` VALUES (5, 2, 20350, 1000);
INSERT INTO `carrier_pos` VALUES (6, 2, 40350, 5135);

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
  CONSTRAINT `carrier_id_fk` FOREIGN KEY (`id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `carrier_stock_id_fk` FOREIGN KEY (`stock_id`) REFERENCES `stock` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '配置表-运输车' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config_carrier
-- ----------------------------
INSERT INTO `config_carrier` VALUES (14, b'0', b'0', 0, NULL, 196, NULL, 1305, 30102);
INSERT INTO `config_carrier` VALUES (15, b'0', b'0', 0, NULL, 196, NULL, 10370, 30198);
INSERT INTO `config_carrier` VALUES (16, NULL, NULL, NULL, NULL, 196, NULL, 1305, 32002);
INSERT INTO `config_carrier` VALUES (17, b'0', b'0', 0, NULL, 196, NULL, 10370, 32098);
INSERT INTO `config_carrier` VALUES (18, b'0', b'0', 0, NULL, 196, NULL, 1305, 30802);
INSERT INTO `config_carrier` VALUES (19, b'0', b'0', 0, NULL, 196, NULL, 10370, 30898);
INSERT INTO `config_carrier` VALUES (20, b'0', b'0', 0, NULL, 196, NULL, 1305, 30402);
INSERT INTO `config_carrier` VALUES (21, b'0', b'0', 0, NULL, 196, NULL, 10370, 30498);
INSERT INTO `config_carrier` VALUES (22, b'0', b'0', 0, NULL, 196, NULL, 1305, 31802);
INSERT INTO `config_carrier` VALUES (23, b'0', b'0', 0, NULL, 196, NULL, 10370, 32898);
INSERT INTO `config_carrier` VALUES (30, b'0', b'0', 0, NULL, 196, NULL, 1305, 35602);
INSERT INTO `config_carrier` VALUES (31, b'0', b'0', 0, NULL, 196, NULL, 1305, 36102);
INSERT INTO `config_carrier` VALUES (32, NULL, NULL, NULL, NULL, 196, NULL, 4846, 36698);

-- ----------------------------
-- Table structure for config_ferry
-- ----------------------------
DROP TABLE IF EXISTS `config_ferry`;
CREATE TABLE `config_ferry`  (
  `id` int(11) UNSIGNED NOT NULL COMMENT '摆渡车设备ID',
  `track_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车轨道ID',
  `track_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡车轨道地标',
  `sim_init_point` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '模拟初始化对轨值',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `ferry_track_id_index`(`track_id`) USING BTREE,
  CONSTRAINT `ferry__id_fk` FOREIGN KEY (`id`) REFERENCES `device` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `ferry_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '配置表-摆渡车' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config_ferry
-- ----------------------------
INSERT INTO `config_ferry` VALUES (10, 75, 20150, NULL);
INSERT INTO `config_ferry` VALUES (11, 76, 20250, NULL);
INSERT INTO `config_ferry` VALUES (12, 77, 40150, NULL);
INSERT INTO `config_ferry` VALUES (13, 78, 40250, NULL);
INSERT INTO `config_ferry` VALUES (28, 109, 20350, NULL);
INSERT INTO `config_ferry` VALUES (29, 110, 40350, NULL);

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
INSERT INTO `config_tilelifter` VALUES (1, 0, 1, 101, 2, 102, 2, 0, 0, 21, 3, 40, 0, b'1', b'0', 2, 0, b'0', b'0', NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (2, 0, 3, 103, 4, 104, 2, 0, 0, 23, 0, 3, 1, b'0', b'0', 2, 0, b'0', b'0', NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (3, 0, 5, 105, 6, 106, 2, 0, 0, 23, 0, 3, 1, b'0', b'0', 2, 0, b'0', b'1', '1,2,4', 2, 1);
INSERT INTO `config_tilelifter` VALUES (4, 0, 7, 107, 8, 108, 2, 0, 0, 36, 0, 38, 0, b'0', b'0', 2, 0, b'0', b'0', NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (5, 0, 9, 109, 10, 110, 2, 0, 0, 44, 0, 37, 0, b'0', b'0', 2, 0, b'0', b'1', '6', NULL, 1);
INSERT INTO `config_tilelifter` VALUES (6, 0, 11, 111, 12, 112, 2, 0, 0, 45, 0, 39, 0, b'0', b'0', 2, 0, b'0', b'0', NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (7, 0, 13, 501, 14, 502, 0, 2, 0, 0, 0, 1, 0, b'0', b'0', 1, 0, b'0', b'0', NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (8, 0, 15, 503, 16, 504, 0, 2, 0, 0, 19, 36, 0, b'1', b'0', 1, 0, b'0', b'0', NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (9, 0, 17, 505, 18, 505, 0, 2, 0, 0, 0, 1, 0, b'0', b'0', 1, 0, b'0', b'0', NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (24, 0, 79, 121, 80, 122, 2, 0, 0, 0, 0, 23, 0, b'0', b'0', 2, 0, b'0', NULL, NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (25, 0, 81, 123, 82, 124, 2, 0, 0, 0, 0, 23, 0, b'0', b'0', 2, 0, b'0', NULL, NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (26, 0, 83, 125, 84, 126, 2, 0, 0, 105, 0, 26, 0, b'0', b'0', 2, 0, b'0', NULL, NULL, NULL, 1);
INSERT INTO `config_tilelifter` VALUES (27, 0, 85, 511, 86, 512, 0, 2, 0, 0, 0, 23, 0, b'0', b'0', 1, 0, b'0', NULL, NULL, NULL, 1);

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
) ENGINE = InnoDB AUTO_INCREMENT = 2173 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '库存消耗记录表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '设备表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of device
-- ----------------------------
INSERT INTO `device` VALUES (1, '3A11', '127.0.0.1', 2001, 1, 2, b'1', 0, 0, '161', 1, 3, b'1');
INSERT INTO `device` VALUES (2, '3A12', '127.0.0.1', 2001, 1, 1, b'1', 0, 0, '162', 1, 3, b'1');
INSERT INTO `device` VALUES (3, '3A13（备用）', '127.0.0.1', 2001, 1, 2, b'1', 0, 0, '163', 1, 3, b'1');
INSERT INTO `device` VALUES (4, '1A11', '127.0.0.1', 2001, 1, 2, b'1', 0, 0, '164', 1, 1, b'1');
INSERT INTO `device` VALUES (5, '1A12（备用）', '127.0.0.1', 2001, 1, 2, b'1', 0, 0, '165', 1, 2, b'1');
INSERT INTO `device` VALUES (6, '2A11', '127.0.0.1', 2001, 1, 2, b'1', 0, 0, '166', 1, 2, b'1');
INSERT INTO `device` VALUES (7, '3D11', '127.0.0.1', 2001, 0, 2, b'1', 0, 0, '209', 1, 3, b'1');
INSERT INTO `device` VALUES (8, '1D11', '127.0.0.1', 2001, 0, 2, b'1', 0, 0, '210', 1, 1, b'1');
INSERT INTO `device` VALUES (9, '2D11', '127.0.0.1', 2001, 0, 2, b'1', 0, 0, '211', 1, 2, b'1');
INSERT INTO `device` VALUES (10, '1B11', '127.0.0.1', 2002, 3, 0, b'1', 0, 0, '177', 1, NULL, b'1');
INSERT INTO `device` VALUES (11, '1B12', '127.0.0.1', 2002, 3, 0, b'1', 0, 0, '178', 1, NULL, b'1');
INSERT INTO `device` VALUES (12, '1B31', '127.0.0.1', 2002, 2, 0, b'1', 0, 0, '181', 1, NULL, b'0');
INSERT INTO `device` VALUES (13, '1B32', '127.0.0.1', 2002, 2, 0, b'1', 0, 0, '182', 1, NULL, b'1');
INSERT INTO `device` VALUES (14, '1C11', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '193', 1, NULL, b'1');
INSERT INTO `device` VALUES (15, '1C12', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '194', 1, NULL, b'1');
INSERT INTO `device` VALUES (16, '1C13', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '195', 1, NULL, b'1');
INSERT INTO `device` VALUES (17, '1C14', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '196', 1, NULL, b'1');
INSERT INTO `device` VALUES (18, '1C15', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '197', 1, NULL, b'1');
INSERT INTO `device` VALUES (19, '1C16', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '198', 1, NULL, b'1');
INSERT INTO `device` VALUES (20, '1C17', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '199', 1, NULL, b'1');
INSERT INTO `device` VALUES (21, '1C18', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '200', 1, NULL, b'1');
INSERT INTO `device` VALUES (22, '1C19', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '201', 1, NULL, b'1');
INSERT INTO `device` VALUES (23, '1C1A', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '202', 1, NULL, b'1');
INSERT INTO `device` VALUES (24, 'AA11', '127.0.0.1', 2001, 1, 2, b'1', 0, 0, '167', 2, NULL, b'1');
INSERT INTO `device` VALUES (25, 'AA12', '127.0.0.1', 2001, 1, 2, b'1', 0, 0, '168', 2, NULL, b'1');
INSERT INTO `device` VALUES (26, 'AA13', '127.0.0.1', 2001, 1, 2, b'1', 0, 0, '169', 2, NULL, b'1');
INSERT INTO `device` VALUES (27, 'AD11', '127.0.0.1', 2001, 0, 2, b'1', 0, 0, '212', 2, NULL, b'1');
INSERT INTO `device` VALUES (28, 'AB11', '127.0.0.1', 2002, 3, 0, b'1', 0, 0, '179', 2, NULL, b'1');
INSERT INTO `device` VALUES (29, 'AB15', '127.0.0.1', 2002, 2, 0, b'1', 0, 0, '183', 2, NULL, b'1');
INSERT INTO `device` VALUES (30, 'AC11', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '203', 2, NULL, b'1');
INSERT INTO `device` VALUES (31, 'AC12', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '204', 2, NULL, b'1');
INSERT INTO `device` VALUES (32, 'AC13', '127.0.0.1', 2003, 4, 0, b'1', 0, 0, '205', 2, NULL, b'1');

-- ----------------------------
-- Table structure for device_copy1
-- ----------------------------
DROP TABLE IF EXISTS `device_copy1`;
CREATE TABLE `device_copy1`  (
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
) ENGINE = InnoDB AUTO_INCREMENT = 32 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of device_copy1
-- ----------------------------
INSERT INTO `device_copy1` VALUES (1, '1A01', '192.168.0.31', 2000, 1, 2, b'1', 0, 0, '161', 1, b'1');
INSERT INTO `device_copy1` VALUES (2, '1A02', '192.168.0.36', 2000, 1, 2, b'1', 0, 0, '162', 1, b'1');
INSERT INTO `device_copy1` VALUES (3, '1A03（备用）', '192.168.0.41', 2000, 1, 2, b'1', 0, 0, '163', 1, b'0');
INSERT INTO `device_copy1` VALUES (4, '1A04', '192.168.0.46', 2000, 1, 2, b'1', 0, 0, '164', 1, b'1');
INSERT INTO `device_copy1` VALUES (5, '1A05（备用）', '192.168.0.51', 2000, 1, 2, b'1', 0, 0, '165', 1, b'1');
INSERT INTO `device_copy1` VALUES (6, '1A06', '192.168.0.56', 2000, 1, 2, b'1', 0, 0, '166', 1, b'1');
INSERT INTO `device_copy1` VALUES (7, '3D11', '192.168.0.81', 2000, 0, 2, b'1', 0, 0, '209', 1, b'1');
INSERT INTO `device_copy1` VALUES (8, '1D11', '192.168.0.86', 2000, 0, 2, b'1', 0, 0, '210', 1, b'1');
INSERT INTO `device_copy1` VALUES (9, '2D11', '192.168.0.91', 2000, 0, 2, b'1', 0, 0, '211', 1, b'1');
INSERT INTO `device_copy1` VALUES (10, '1B11', '192.168.0.131', 2000, 3, 0, b'1', 0, 0, '177', 1, b'1');
INSERT INTO `device_copy1` VALUES (11, '1B12', '192.168.0.132', 2000, 3, 0, b'1', 0, 0, '178', 1, b'1');
INSERT INTO `device_copy1` VALUES (12, '1B31', '192.168.0.135', 2000, 2, 0, b'1', 0, 0, '181', 1, b'1');
INSERT INTO `device_copy1` VALUES (13, '1B32', '192.168.0.136', 2000, 2, 0, b'1', 0, 0, '182', 1, b'1');
INSERT INTO `device_copy1` VALUES (14, '1C11', '192.168.0.151', 2000, 4, 0, b'1', 0, 0, '193', 1, b'1');
INSERT INTO `device_copy1` VALUES (15, '1C12', '192.168.0.152', 2000, 4, 0, b'1', 0, 0, '194', 1, b'1');
INSERT INTO `device_copy1` VALUES (16, '1C13', '192.168.0.153', 2000, 4, 0, b'1', 0, 0, '195', 1, b'1');
INSERT INTO `device_copy1` VALUES (17, '1C14', '192.168.0.154', 2000, 4, 0, b'1', 0, 0, '196', 1, b'1');
INSERT INTO `device_copy1` VALUES (18, '1C15', '192.168.0.155', 2000, 4, 0, b'1', 0, 0, '197', 1, b'1');
INSERT INTO `device_copy1` VALUES (19, '1C16', '192.168.0.156', 2000, 4, 0, b'1', 0, 0, '198', 1, b'1');
INSERT INTO `device_copy1` VALUES (20, '1C17', '192.168.0.157', 2000, 4, 0, b'1', 0, 0, '199', 1, b'1');
INSERT INTO `device_copy1` VALUES (21, '1C18', '192.168.0.158', 2000, 4, 0, b'1', 0, 0, '200', 1, b'1');
INSERT INTO `device_copy1` VALUES (22, '1C19', '192.168.0.159', 2000, 4, 0, b'1', 0, 0, '201', 1, b'1');
INSERT INTO `device_copy1` VALUES (23, '1C1A', '192.168.0.160', 2000, 4, 0, b'1', 0, 0, '202', 1, b'1');
INSERT INTO `device_copy1` VALUES (24, '2A01', '192.168.0.61', 2000, 1, 2, b'1', 0, 0, '167', 2, b'1');
INSERT INTO `device_copy1` VALUES (25, '2A02', '192.168.0.66', 2000, 1, 2, b'1', 0, 0, '168', 2, b'1');
INSERT INTO `device_copy1` VALUES (26, '2A03', '192.168.0.71', 2000, 1, 2, b'1', 0, 0, '169', 2, b'1');
INSERT INTO `device_copy1` VALUES (27, '2D01', '192.168.0.96', 2000, 0, 2, b'1', 0, 0, '212', 2, b'1');
INSERT INTO `device_copy1` VALUES (28, '2B01', '192.168.0.133', 2000, 3, 0, b'1', 0, 0, '179', 2, b'1');
INSERT INTO `device_copy1` VALUES (29, '2B05', '192.168.0.137', 2000, 2, 0, b'1', 0, 0, '183', 2, b'1');
INSERT INTO `device_copy1` VALUES (30, '2C01', '192.168.0.161', 2000, 4, 0, b'1', 0, 0, '203', 2, b'1');
INSERT INTO `device_copy1` VALUES (31, '2C02', '192.168.0.162', 2000, 4, 0, b'1', 0, 0, '204', 2, b'1');
INSERT INTO `device_copy1` VALUES (32, '2C03', '192.168.0.163', 2000, 4, 0, b'1', 0, 0, '205', 2, b'1');

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
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '字典表' ROW_FORMAT = Dynamic;

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
  `double_value` double(10, 3) UNSIGNED NULL DEFAULT NULL COMMENT '浮点类型',
  `uint_value` int(11) UNSIGNED NULL DEFAULT NULL,
  `order` smallint(5) UNSIGNED NULL DEFAULT NULL,
  `updatetime` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `dic_id_fk`(`diction_id`) USING BTREE,
  CONSTRAINT `dic_id_fk` FOREIGN KEY (`diction_id`) REFERENCES `diction` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 217 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '字典明细表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of diction_dtl
-- ----------------------------
INSERT INTO `diction_dtl` VALUES (1, 1, 'NewStockId', '生成库存ID', NULL, NULL, '', NULL, 1648, NULL, '2021-03-19 17:35:00');
INSERT INTO `diction_dtl` VALUES (2, 1, 'NewTranId', '生成交易ID', NULL, NULL, '', NULL, 3780, NULL, '2021-03-19 17:34:58');
INSERT INTO `diction_dtl` VALUES (3, 1, 'NewWarnId', '生成警告ID', NULL, NULL, '', NULL, 12613, NULL, '2021-03-22 17:35:11');
INSERT INTO `diction_dtl` VALUES (4, 1, 'NewGoodId', '生成品种ID', NULL, NULL, '', NULL, 41, NULL, '2021-03-18 09:26:24');
INSERT INTO `diction_dtl` VALUES (5, 1, 'NewTrafficCtlId', '生成交管ID', NULL, NULL, '', NULL, 1, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (11, 2, 'Area1Down', '1号线下砖', NULL, b'1', '', NULL, NULL, NULL, '2021-03-19 17:30:02');
INSERT INTO `diction_dtl` VALUES (12, 2, 'Area1Up', '1号线上砖', NULL, b'0', '', NULL, NULL, NULL, '2021-03-18 08:21:35');
INSERT INTO `diction_dtl` VALUES (13, 2, 'Area1Sort', '1号线倒库', NULL, b'0', '', NULL, NULL, NULL, '2021-03-19 17:16:21');
INSERT INTO `diction_dtl` VALUES (14, 2, 'Area2Down', '2号线下砖', NULL, b'0', '', NULL, NULL, NULL, '2021-03-12 15:44:18');
INSERT INTO `diction_dtl` VALUES (15, 2, 'Area2Up', '2号线上砖', NULL, b'0', '', NULL, NULL, NULL, '2021-03-15 10:16:32');
INSERT INTO `diction_dtl` VALUES (40, 9, 'GoodLevel', '全捡混砖', 0, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (41, 9, 'GoodLevel', '优等品', 1, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (42, 9, 'GoodLevel', '一级品', 2, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (43, 9, 'GoodLevel', '二级品', 3, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (44, 9, 'GoodLevel', '合格品', 4, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (50, 4, 'MinStockTime', '最小存放时间(小时)', 0, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (51, 5, 'FerryAvoidNumber', '摆渡车(轨道数)', 3, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (52, 6, 'PDA_INIT_VERSION', 'PDA基础字典版本', 10, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (53, 6, 'PDA_GOOD_VERSION', 'PDA规格字典版本', 59, NULL, '', NULL, NULL, NULL, '2021-03-18 09:26:24');
INSERT INTO `diction_dtl` VALUES (54, 7, 'TileLifterShiftCount', '下砖机转产差值(层数)', 99, NULL, '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (55, 8, 'UserLoginFunction', 'PDA登陆功能开关', NULL, b'1', '', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (56, 10, 'Pulse2MM', '1脉冲=毫米', NULL, NULL, '', 17.360, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (57, 10, 'Pulse2CM', '1脉冲=厘米', NULL, NULL, '', 1.736, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (58, 10, 'StackPluse', '一垛计算距离', NULL, NULL, '', 217.000, NULL, NULL, '2021-03-18 11:19:51');
INSERT INTO `diction_dtl` VALUES (59, 8, 'TileNeedSysShiftFunc', '砖机需转产信号', NULL, b'0', '', NULL, NULL, NULL, '2021-03-19 18:50:27');
INSERT INTO `diction_dtl` VALUES (60, 8, 'AutoBackupTileFunc', '备用砖机自动转换', NULL, b'0', '', NULL, NULL, NULL, '2021-03-19 18:51:14');
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
INSERT INTO `diction_dtl` VALUES (110, 3, 'WarningA2X2', '下降到位信号异常', NULL, NULL, '下降到位信号异常', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (111, 3, 'WarningA2X3', '小车检测到无砖', NULL, NULL, '小车检测到无砖', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (112, 3, 'WarningA2X4', '顶升超时，上升到位信号异常', NULL, NULL, '顶升超时，上升到位信号异常', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (113, 3, 'WarningA2X5', '下降超时，下降到位信号异常', NULL, NULL, '下降超时，下降到位信号异常', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (114, 3, 'WarningA2X6', '运输车倒库无砖', NULL, NULL, '运输车倒库无砖', NULL, NULL, NULL, NULL);
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
INSERT INTO `diction_dtl` VALUES (215, 3, 'TileGoodsIsZero', '砖机工位品种反馈异常', NULL, NULL, '砖机工位品种反馈异常，尝试使用PC当前品种', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (216, 3, 'TileGoodsIsNull', '砖机工位品种没有配置', NULL, NULL, '砖机工位品种没有配置，尝试使用PC当前品种', NULL, NULL, NULL, NULL);
INSERT INTO `diction_dtl` VALUES (217, 3, 'TransHaveNotTheGiveTrack', '任务进行中没有发现合适的轨道卸砖', NULL, NULL, '任务中没有合适轨道卸砖', NULL, NULL, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 512 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '摆渡车对位表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ferry_pos
-- ----------------------------
INSERT INTO `ferry_pos` VALUES (217, 19, 10, 301, -2409);
INSERT INTO `ferry_pos` VALUES (218, 20, 10, 302, -1892);
INSERT INTO `ferry_pos` VALUES (219, 21, 10, 303, -1374);
INSERT INTO `ferry_pos` VALUES (220, 22, 10, 304, -863);
INSERT INTO `ferry_pos` VALUES (221, 23, 10, 305, 541);
INSERT INTO `ferry_pos` VALUES (222, 24, 10, 306, 1061);
INSERT INTO `ferry_pos` VALUES (223, 25, 10, 307, 1578);
INSERT INTO `ferry_pos` VALUES (224, 26, 10, 308, 2093);
INSERT INTO `ferry_pos` VALUES (225, 27, 10, 309, 2609);
INSERT INTO `ferry_pos` VALUES (226, 28, 10, 310, 3124);
INSERT INTO `ferry_pos` VALUES (227, 29, 10, 311, 3636);
INSERT INTO `ferry_pos` VALUES (228, 30, 10, 312, 4160);
INSERT INTO `ferry_pos` VALUES (229, 31, 10, 313, 4673);
INSERT INTO `ferry_pos` VALUES (230, 32, 10, 314, 5188);
INSERT INTO `ferry_pos` VALUES (231, 33, 10, 315, 5704);
INSERT INTO `ferry_pos` VALUES (232, 34, 10, 316, 6221);
INSERT INTO `ferry_pos` VALUES (233, 35, 10, 317, 6742);
INSERT INTO `ferry_pos` VALUES (234, 36, 10, 318, 7263);
INSERT INTO `ferry_pos` VALUES (235, 37, 10, 319, 7781);
INSERT INTO `ferry_pos` VALUES (236, 38, 10, 320, 8304);
INSERT INTO `ferry_pos` VALUES (237, 39, 10, 321, 8820);
INSERT INTO `ferry_pos` VALUES (238, 40, 10, 322, 9335);
INSERT INTO `ferry_pos` VALUES (239, 41, 10, 323, 9860);
INSERT INTO `ferry_pos` VALUES (240, 42, 10, 324, 10373);
INSERT INTO `ferry_pos` VALUES (241, 43, 10, 325, 10883);
INSERT INTO `ferry_pos` VALUES (242, 44, 10, 326, 11411);
INSERT INTO `ferry_pos` VALUES (243, 1, 10, 101, -2466);
INSERT INTO `ferry_pos` VALUES (244, 2, 10, 102, -1209);
INSERT INTO `ferry_pos` VALUES (245, 3, 10, 103, 765);
INSERT INTO `ferry_pos` VALUES (246, 4, 10, 104, 2011);
INSERT INTO `ferry_pos` VALUES (247, 5, 10, 105, 3541);
INSERT INTO `ferry_pos` VALUES (248, 6, 10, 106, 4788);
INSERT INTO `ferry_pos` VALUES (249, 7, 10, 107, 5988);
INSERT INTO `ferry_pos` VALUES (250, 8, 10, 108, 7240);
INSERT INTO `ferry_pos` VALUES (251, 9, 10, 109, 8444);
INSERT INTO `ferry_pos` VALUES (252, 10, 10, 110, 9693);
INSERT INTO `ferry_pos` VALUES (253, 29, 11, 311, -4938);
INSERT INTO `ferry_pos` VALUES (254, 30, 11, 312, -4415);
INSERT INTO `ferry_pos` VALUES (255, 31, 11, 313, -3901);
INSERT INTO `ferry_pos` VALUES (256, 32, 11, 314, -3387);
INSERT INTO `ferry_pos` VALUES (257, 33, 11, 315, -2870);
INSERT INTO `ferry_pos` VALUES (258, 34, 11, 316, -2354);
INSERT INTO `ferry_pos` VALUES (259, 35, 11, 317, -1832);
INSERT INTO `ferry_pos` VALUES (260, 36, 11, 318, -1311);
INSERT INTO `ferry_pos` VALUES (261, 37, 11, 319, -796);
INSERT INTO `ferry_pos` VALUES (262, 38, 11, 320, -270);
INSERT INTO `ferry_pos` VALUES (263, 39, 11, 321, 252);
INSERT INTO `ferry_pos` VALUES (264, 40, 11, 322, 767);
INSERT INTO `ferry_pos` VALUES (265, 41, 11, 323, 1291);
INSERT INTO `ferry_pos` VALUES (266, 42, 11, 324, 1806);
INSERT INTO `ferry_pos` VALUES (267, 43, 11, 325, 2316);
INSERT INTO `ferry_pos` VALUES (268, 44, 11, 326, 2844);
INSERT INTO `ferry_pos` VALUES (269, 45, 11, 327, 3362);
INSERT INTO `ferry_pos` VALUES (270, 46, 11, 328, 3877);
INSERT INTO `ferry_pos` VALUES (271, 28, 11, 310, -5451);
INSERT INTO `ferry_pos` VALUES (272, 27, 11, 309, -5966);
INSERT INTO `ferry_pos` VALUES (273, 26, 11, 308, -6487);
INSERT INTO `ferry_pos` VALUES (274, 25, 11, 307, -7001);
INSERT INTO `ferry_pos` VALUES (275, 24, 11, 306, -7518);
INSERT INTO `ferry_pos` VALUES (276, 23, 11, 305, -8038);
INSERT INTO `ferry_pos` VALUES (277, 22, 11, 304, -9441);
INSERT INTO `ferry_pos` VALUES (278, 21, 11, 303, -9952);
INSERT INTO `ferry_pos` VALUES (279, 7, 11, 107, -2592);
INSERT INTO `ferry_pos` VALUES (280, 8, 11, 108, -1341);
INSERT INTO `ferry_pos` VALUES (281, 9, 11, 109, -136);
INSERT INTO `ferry_pos` VALUES (282, 10, 11, 110, 1113);
INSERT INTO `ferry_pos` VALUES (283, 11, 11, 111, 2311);
INSERT INTO `ferry_pos` VALUES (284, 12, 11, 112, 3568);
INSERT INTO `ferry_pos` VALUES (285, 6, 11, 106, -3789);
INSERT INTO `ferry_pos` VALUES (286, 5, 11, 105, -5038);
INSERT INTO `ferry_pos` VALUES (287, 4, 11, 104, -6566);
INSERT INTO `ferry_pos` VALUES (288, 3, 11, 103, -7814);
INSERT INTO `ferry_pos` VALUES (289, 57, 13, 311, -4921);
INSERT INTO `ferry_pos` VALUES (290, 58, 13, 312, -4406);
INSERT INTO `ferry_pos` VALUES (291, 59, 13, 313, -3893);
INSERT INTO `ferry_pos` VALUES (292, 60, 13, 314, -3376);
INSERT INTO `ferry_pos` VALUES (293, 61, 13, 315, -2857);
INSERT INTO `ferry_pos` VALUES (294, 62, 13, 316, -2342);
INSERT INTO `ferry_pos` VALUES (295, 63, 13, 317, -1821);
INSERT INTO `ferry_pos` VALUES (296, 64, 13, 318, -1305);
INSERT INTO `ferry_pos` VALUES (297, 65, 13, 319, -788);
INSERT INTO `ferry_pos` VALUES (298, 66, 13, 320, -261);
INSERT INTO `ferry_pos` VALUES (299, 67, 13, 321, 254);
INSERT INTO `ferry_pos` VALUES (300, 68, 13, 322, 777);
INSERT INTO `ferry_pos` VALUES (301, 69, 13, 323, 1292);
INSERT INTO `ferry_pos` VALUES (302, 70, 13, 324, 1806);
INSERT INTO `ferry_pos` VALUES (303, 71, 13, 325, 2323);
INSERT INTO `ferry_pos` VALUES (304, 72, 13, 326, 2847);
INSERT INTO `ferry_pos` VALUES (305, 73, 13, 327, 3361);
INSERT INTO `ferry_pos` VALUES (306, 74, 13, 328, 3877);
INSERT INTO `ferry_pos` VALUES (307, 56, 13, 310, -5445);
INSERT INTO `ferry_pos` VALUES (308, 55, 13, 309, -5960);
INSERT INTO `ferry_pos` VALUES (309, 54, 13, 308, -6482);
INSERT INTO `ferry_pos` VALUES (310, 53, 13, 307, -6995);
INSERT INTO `ferry_pos` VALUES (311, 52, 13, 306, -7512);
INSERT INTO `ferry_pos` VALUES (312, 51, 13, 305, -8028);
INSERT INTO `ferry_pos` VALUES (313, 50, 13, 304, -9423);
INSERT INTO `ferry_pos` VALUES (314, 49, 13, 303, -9941);
INSERT INTO `ferry_pos` VALUES (315, 15, 13, 503, -3458);
INSERT INTO `ferry_pos` VALUES (316, 16, 13, 504, -2202);
INSERT INTO `ferry_pos` VALUES (317, 17, 13, 505, 1178);
INSERT INTO `ferry_pos` VALUES (318, 18, 13, 506, 2428);
INSERT INTO `ferry_pos` VALUES (319, 13, 13, 501, -6898);
INSERT INTO `ferry_pos` VALUES (320, 14, 13, 502, -5646);
INSERT INTO `ferry_pos` VALUES (321, 47, 12, 301, -2058);
INSERT INTO `ferry_pos` VALUES (322, 48, 12, 302, -1542);
INSERT INTO `ferry_pos` VALUES (323, 49, 12, 303, -1024);
INSERT INTO `ferry_pos` VALUES (324, 50, 12, 304, -505);
INSERT INTO `ferry_pos` VALUES (325, 51, 12, 305, 894);
INSERT INTO `ferry_pos` VALUES (326, 52, 12, 306, 1410);
INSERT INTO `ferry_pos` VALUES (327, 53, 12, 307, 1926);
INSERT INTO `ferry_pos` VALUES (328, 54, 12, 308, 2439);
INSERT INTO `ferry_pos` VALUES (329, 55, 12, 309, 2961);
INSERT INTO `ferry_pos` VALUES (330, 56, 12, 310, 3475);
INSERT INTO `ferry_pos` VALUES (331, 57, 12, 311, 4000);
INSERT INTO `ferry_pos` VALUES (332, 58, 12, 312, 4515);
INSERT INTO `ferry_pos` VALUES (333, 59, 12, 313, 5028);
INSERT INTO `ferry_pos` VALUES (334, 60, 12, 314, 5546);
INSERT INTO `ferry_pos` VALUES (335, 61, 12, 315, 6064);
INSERT INTO `ferry_pos` VALUES (336, 62, 12, 316, 6579);
INSERT INTO `ferry_pos` VALUES (337, 63, 12, 317, 7104);
INSERT INTO `ferry_pos` VALUES (338, 64, 12, 318, 7617);
INSERT INTO `ferry_pos` VALUES (339, 65, 12, 319, 8134);
INSERT INTO `ferry_pos` VALUES (340, 66, 12, 320, 8660);
INSERT INTO `ferry_pos` VALUES (341, 67, 12, 321, 9175);
INSERT INTO `ferry_pos` VALUES (342, 68, 12, 322, 9696);
INSERT INTO `ferry_pos` VALUES (343, 69, 12, 323, 10212);
INSERT INTO `ferry_pos` VALUES (344, 70, 12, 324, 10727);
INSERT INTO `ferry_pos` VALUES (345, 71, 12, 325, 11243);
INSERT INTO `ferry_pos` VALUES (346, 72, 12, 326, 11772);
INSERT INTO `ferry_pos` VALUES (347, 13, 12, 501, 2024);
INSERT INTO `ferry_pos` VALUES (348, 14, 12, 502, 3275);
INSERT INTO `ferry_pos` VALUES (349, 15, 12, 503, 5462);
INSERT INTO `ferry_pos` VALUES (350, 16, 12, 504, 6721);
INSERT INTO `ferry_pos` VALUES (351, 17, 12, 505, 10099);
INSERT INTO `ferry_pos` VALUES (352, 18, 12, 506, 11349);
INSERT INTO `ferry_pos` VALUES (353, 87, 28, 351, -8916);
INSERT INTO `ferry_pos` VALUES (355, 88, 28, 352, -8151);
INSERT INTO `ferry_pos` VALUES (357, 89, 28, 353, -7399);
INSERT INTO `ferry_pos` VALUES (359, 90, 28, 354, -6627);
INSERT INTO `ferry_pos` VALUES (361, 91, 28, 355, -5867);
INSERT INTO `ferry_pos` VALUES (363, 92, 28, 356, -5106);
INSERT INTO `ferry_pos` VALUES (365, 93, 28, 357, -4341);
INSERT INTO `ferry_pos` VALUES (367, 94, 28, 358, -3567);
INSERT INTO `ferry_pos` VALUES (369, 95, 28, 359, -2800);
INSERT INTO `ferry_pos` VALUES (371, 96, 28, 360, -2039);
INSERT INTO `ferry_pos` VALUES (373, 97, 28, 361, -1284);
INSERT INTO `ferry_pos` VALUES (375, 98, 28, 362, -521);
INSERT INTO `ferry_pos` VALUES (377, 99, 28, 363, 245);
INSERT INTO `ferry_pos` VALUES (379, 100, 28, 364, 1005);
INSERT INTO `ferry_pos` VALUES (381, 101, 28, 365, 1772);
INSERT INTO `ferry_pos` VALUES (383, 102, 28, 366, 2537);
INSERT INTO `ferry_pos` VALUES (385, 103, 28, 367, 4278);
INSERT INTO `ferry_pos` VALUES (387, 104, 28, 368, 5044);
INSERT INTO `ferry_pos` VALUES (389, 105, 28, 369, 5804);
INSERT INTO `ferry_pos` VALUES (391, 106, 28, 370, 6574);
INSERT INTO `ferry_pos` VALUES (393, 107, 28, 371, 7340);
INSERT INTO `ferry_pos` VALUES (395, 108, 28, 372, 8104);
INSERT INTO `ferry_pos` VALUES (397, 79, 28, 121, -2916);
INSERT INTO `ferry_pos` VALUES (399, 80, 28, 122, -1383);
INSERT INTO `ferry_pos` VALUES (401, 81, 28, 123, 364);
INSERT INTO `ferry_pos` VALUES (403, 82, 28, 124, 1890);
INSERT INTO `ferry_pos` VALUES (405, 83, 28, 125, 4724);
INSERT INTO `ferry_pos` VALUES (407, 84, 28, 126, 6254);
INSERT INTO `ferry_pos` VALUES (409, 87, 29, 351, -8027);
INSERT INTO `ferry_pos` VALUES (411, 88, 29, 352, -7264);
INSERT INTO `ferry_pos` VALUES (413, 89, 29, 353, -6499);
INSERT INTO `ferry_pos` VALUES (415, 90, 29, 354, -5736);
INSERT INTO `ferry_pos` VALUES (417, 91, 29, 355, -4975);
INSERT INTO `ferry_pos` VALUES (419, 92, 29, 356, -4209);
INSERT INTO `ferry_pos` VALUES (421, 93, 29, 357, -3446);
INSERT INTO `ferry_pos` VALUES (423, 94, 29, 358, -2679);
INSERT INTO `ferry_pos` VALUES (425, 95, 29, 359, -1916);
INSERT INTO `ferry_pos` VALUES (427, 96, 29, 360, -1158);
INSERT INTO `ferry_pos` VALUES (429, 97, 29, 361, -397);
INSERT INTO `ferry_pos` VALUES (431, 98, 29, 362, 366);
INSERT INTO `ferry_pos` VALUES (433, 99, 29, 363, 1133);
INSERT INTO `ferry_pos` VALUES (435, 100, 29, 364, 1892);
INSERT INTO `ferry_pos` VALUES (437, 101, 29, 365, 2659);
INSERT INTO `ferry_pos` VALUES (439, 102, 29, 366, 3420);
INSERT INTO `ferry_pos` VALUES (441, 103, 29, 367, 5170);
INSERT INTO `ferry_pos` VALUES (443, 104, 29, 368, 5934);
INSERT INTO `ferry_pos` VALUES (445, 105, 29, 369, 6697);
INSERT INTO `ferry_pos` VALUES (447, 106, 29, 370, 7456);
INSERT INTO `ferry_pos` VALUES (449, 107, 29, 371, 8221);
INSERT INTO `ferry_pos` VALUES (451, 108, 29, 372, 8984);
INSERT INTO `ferry_pos` VALUES (453, 85, 29, 511, 150);
INSERT INTO `ferry_pos` VALUES (455, 86, 29, 512, 1675);

-- ----------------------------
-- Table structure for good_size
-- ----------------------------
DROP TABLE IF EXISTS `good_size`;
CREATE TABLE `good_size`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '规格ID',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '规格名称',
  `length` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '长',
  `width` smallint(5) NULL DEFAULT NULL COMMENT '宽',
  `stack` tinyint(3) UNSIGNED NULL DEFAULT NULL COMMENT '垛',
  `car_lenght` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '一车砖的长度（脉冲）',
  `car_space` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '砖与砖安全间距（脉冲）',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '品种规格表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of good_size
-- ----------------------------
INSERT INTO `good_size` VALUES (1, '800x800', 800, 800, 4, NULL, NULL);
INSERT INTO `good_size` VALUES (2, '600x1200', 1200, 600, 3, NULL, NULL);
INSERT INTO `good_size` VALUES (3, '1200x600', 600, 1200, 5, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 40 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '品种表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of goods
-- ----------------------------
INSERT INTO `goods` VALUES (1, 1, 'bg123', '123', 45, 0, '', '2021-03-06 18:59:25', NULL, 1, 1, '2021-01-30 09:29:05', NULL, NULL, 'bg123/123/800x800/优等品');
INSERT INTO `goods` VALUES (2, 1, 'kyz123', 'a', 45, 0, '', '2021-03-06 18:48:06', NULL, 1, 0, '2021-03-06 18:48:06', NULL, NULL, 'kyz123/a/全捡混砖');
INSERT INTO `goods` VALUES (3, 1, 'B', 'B', 45, 0, '', '2021-03-06 12:20:11', NULL, 1, 1, '2021-01-30 09:30:05', NULL, NULL, 'B/B/800x800/优等品');
INSERT INTO `goods` VALUES (6, 1, 'ww88', '66', 200, 0, '', '2021-03-06 20:48:39', NULL, 1, 0, '2021-03-06 20:48:39', NULL, NULL, 'ww88/66/全捡混砖');
INSERT INTO `goods` VALUES (7, 1, 'zz', '123', 180, 0, '', '2021-03-07 02:00:54', NULL, 1, 0, '2021-03-07 02:00:54', NULL, NULL, 'zz/123/全捡混砖');
INSERT INTO `goods` VALUES (8, 1, 'wwo', '2', 200, 0, '', '2021-03-07 04:59:02', NULL, 1, 3, '2021-03-07 04:59:02', NULL, NULL, 'wwo/2/二级品');
INSERT INTO `goods` VALUES (9, 1, 'kyz', '16', 180, 0, '', '2021-03-07 07:46:02', NULL, 1, 2, '2021-03-07 07:46:02', NULL, NULL, 'kyz/16/一级品');
INSERT INTO `goods` VALUES (10, 1, 'HT13809', '10', 200, 0, '', '2021-03-08 02:07:12', NULL, 1, 1, '2021-03-08 02:07:12', NULL, NULL, 'HT13809/10/优等品');
INSERT INTO `goods` VALUES (12, 1, 'HT13801', '9', 45, 0, '', '2021-03-08 17:28:16', NULL, 1, 1, '2021-03-08 17:28:16', NULL, NULL, 'HT13801/9/优等品');
INSERT INTO `goods` VALUES (13, 1, 'JT13908', '03', 180, 0, '', '2021-03-09 00:05:02', NULL, 1, 1, '2021-03-09 00:05:02', NULL, NULL, 'JT13908/03/优等品');
INSERT INTO `goods` VALUES (14, 1, 'JT13910', '8', 45, 0, '', '2021-03-09 07:46:49', NULL, 1, 1, '2021-03-09 07:46:49', NULL, NULL, 'JT13910/8/优等品');
INSERT INTO `goods` VALUES (15, 1, 'MHA13618', '7', 45, 0, '', '2021-03-09 13:00:11', NULL, 1, 1, '2021-03-09 13:00:11', NULL, NULL, 'MHA13618/7/优等品');
INSERT INTO `goods` VALUES (16, 1, '13615', '123', 180, 0, '', '2021-03-10 03:15:58', NULL, 1, 0, '2021-03-10 03:15:58', NULL, NULL, '13615/123/全捡混砖');
INSERT INTO `goods` VALUES (17, 1, 'MHA13616', '6', 45, 0, '', '2021-03-10 14:33:13', NULL, 1, 1, '2021-03-10 14:33:13', NULL, NULL, 'MHA13616/6/优等品');
INSERT INTO `goods` VALUES (18, 1, 'MHA13606', '03', 200, 0, '', '2021-03-10 21:22:57', NULL, 1, 1, '2021-03-10 21:22:57', NULL, NULL, 'MHA13606/03/优等品');
INSERT INTO `goods` VALUES (19, 1, 'GF81308', '03', 200, 0, '', '2021-03-11 03:45:19', NULL, 1, 1, '2021-03-11 03:45:19', NULL, NULL, 'GF81308/03/优等品');
INSERT INTO `goods` VALUES (20, 1, 'GF81307', '5', 45, 0, '', '2021-03-11 11:05:10', NULL, 1, 1, '2021-03-11 11:05:10', NULL, NULL, 'GF81307/5/优等品');
INSERT INTO `goods` VALUES (22, 1, 'A', 'A', 45, 0, '', '2021-03-11 11:48:04', NULL, 2, 1, '2021-03-11 11:48:04', NULL, NULL, 'A/A/600x1200/优等品');
INSERT INTO `goods` VALUES (23, 2, 'A', 'A', 45, 0, '', '2021-03-11 15:32:22', NULL, 3, 1, '2021-03-11 15:32:22', NULL, NULL, 'A/A/1200x600/优等品');
INSERT INTO `goods` VALUES (24, 1, 'GF81311', '2', 45, 0, '', '2021-03-12 14:10:40', NULL, 1, 1, '2021-03-12 14:10:40', NULL, NULL, 'GF81311/2/优等品');
INSERT INTO `goods` VALUES (25, 1, 'wfh', '1', 45, 0, '', '2021-03-12 14:21:53', NULL, 1, 1, '2021-03-12 14:21:53', NULL, NULL, 'wfh/1/优等品');
INSERT INTO `goods` VALUES (26, 2, 'B', 'B', 45, 0, '', '2021-03-12 15:10:21', NULL, 3, 2, '2021-03-12 15:10:21', NULL, NULL, 'B/B/1200x600/一级品');
INSERT INTO `goods` VALUES (27, 1, 'gj136', 'p', 45, 0, '', '2021-03-13 08:21:59', NULL, 1, 0, '2021-03-13 08:21:59', NULL, NULL, 'gj136/p/全捡混砖');
INSERT INTO `goods` VALUES (28, 1, 'tj68', 'h', 45, 0, '', '2021-03-14 07:07:34', NULL, 1, 0, '2021-03-14 07:07:34', NULL, NULL, 'tj68/h/全捡混砖');
INSERT INTO `goods` VALUES (29, 1, 'ty468', 'u', 45, 0, '', '2021-03-14 07:13:16', NULL, 1, 0, '2021-03-14 07:13:16', NULL, NULL, 'ty468/u/全捡混砖');
INSERT INTO `goods` VALUES (30, 1, 'GF81313', '4', 45, 0, '', '2021-03-14 11:58:38', NULL, 1, 1, '2021-03-14 11:58:38', NULL, NULL, 'GF81313/4/优等品');
INSERT INTO `goods` VALUES (31, 1, 'Rg468', 'g', 45, 0, '', '2021-03-14 12:54:41', NULL, 1, 1, '2021-03-14 12:54:41', NULL, NULL, 'Rg468/g/优等品');
INSERT INTO `goods` VALUES (32, 1, 'GT6', 'g', 45, 0, '', '2021-03-14 13:36:33', NULL, 1, 1, '2021-03-14 13:36:33', NULL, NULL, 'GT6/g/优等品');
INSERT INTO `goods` VALUES (33, 1, 'G7', 'g', 45, 0, '', '2021-03-14 13:42:43', NULL, 1, 1, '2021-03-14 13:42:43', NULL, NULL, 'G7/g/优等品');
INSERT INTO `goods` VALUES (34, 1, '12J02', '1', 50, 0, '', '2021-03-14 17:01:00', NULL, 2, 0, '2021-03-14 17:01:00', NULL, NULL, '12J02/1/全捡混砖');
INSERT INTO `goods` VALUES (35, 1, 'GF81309', '2', 45, 0, '', '2021-03-14 19:10:17', NULL, 1, 1, '2021-03-14 19:10:17', NULL, NULL, 'GF81309/2/优等品');
INSERT INTO `goods` VALUES (36, 1, 'HM8803', '03', 200, 0, '', '2021-03-15 15:21:43', NULL, 1, 1, '2021-03-15 15:21:43', NULL, NULL, 'HM8803/03/优等品');
INSERT INTO `goods` VALUES (37, 1, '12J06', '2', 50, 0, '', '2021-03-15 22:10:33', NULL, 2, 1, '2021-03-15 22:10:33', NULL, NULL, '12J06/2/优等品');
INSERT INTO `goods` VALUES (38, 1, 'K8896', 'h', 45, 0, '', '2021-03-16 04:17:48', NULL, 1, 1, '2021-03-16 04:17:48', NULL, NULL, 'K8896/h/优等品');
INSERT INTO `goods` VALUES (39, 1, 'J12307', '3', 200, 0, '', '2021-03-16 12:04:56', NULL, 2, 1, '2021-03-16 12:04:56', NULL, NULL, 'J12307/3/优等品');
INSERT INTO `goods` VALUES (40, 1, 'C', 'C', 45, 0, '自动生成', '2021-03-18 09:26:24', NULL, 1, 1, '2021-03-18 09:26:24', NULL, NULL, 'C/C/800x800/优等品');

-- ----------------------------
-- Table structure for line
-- ----------------------------
DROP TABLE IF EXISTS `line`;
CREATE TABLE `line`  (
  `id` smallint(5) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '序列号',
  `area_id` int(11) UNSIGNED NOT NULL COMMENT '区域ID',
  `line` smallint(5) UNSIGNED NOT NULL COMMENT '线',
  `name` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `sort_task_qty` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '倒库任务数量',
  `up_task_qty` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '上砖任务数量',
  `down_task_qty` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '下砖任务数量',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of line
-- ----------------------------
INSERT INTO `line` VALUES (1, 1, 1, '窑后1线', 1, 1, 1);
INSERT INTO `line` VALUES (2, 1, 2, '窑后2线', 1, 1, 1);
INSERT INTO `line` VALUES (3, 1, 3, '窑后3线', 1, 1, 1);

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
INSERT INTO `rf_client` VALUES ('861563040011541', '', '192.168.0.9:34000', '2021-02-03 14:53:15', '2021-02-03 17:05:22', NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `rf_client` VALUES ('869448031110985', '', '10.9.31.101:44278', '2021-03-18 14:52:34', '2021-03-19 17:53:55', NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `rf_client` VALUES ('869448031111116', '', '192.168.0.8:43512', '2021-03-06 09:07:39', '2021-03-16 07:31:27', NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `rf_client` VALUES ('869448031111223', '', '192.168.0.9:38922', '2021-03-15 11:16:35', '2021-03-15 11:17:02', NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `rf_client` VALUES ('869448031113351', '', '192.168.0.7:47552', '2021-03-01 15:10:28', '2021-03-12 15:04:51', NULL, NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for sim_ferry_pos
-- ----------------------------
DROP TABLE IF EXISTS `sim_ferry_pos`;
CREATE TABLE `sim_ferry_pos`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `area_id` int(11) UNSIGNED NULL DEFAULT NULL COMMENT '设备ID',
  `isdownferry` bit(1) NULL DEFAULT NULL COMMENT '是否是下砖摆渡车',
  `ferry_code` smallint(5) UNSIGNED NULL DEFAULT NULL COMMENT '摆渡编码',
  `ferry_pos` int(11) NULL DEFAULT NULL COMMENT '实际坐标',
  `isdownside` bit(1) NULL DEFAULT NULL COMMENT '是否是下砖测',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sim_ferry_pos
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
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `sto_goods_id_fk`(`goods_id`) USING BTREE,
  INDEX `sto_track_id_fk`(`track_id`) USING BTREE,
  CONSTRAINT `sto_goods_id_fk` FOREIGN KEY (`goods_id`) REFERENCES `goods` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `sto_track_id_fk` FOREIGN KEY (`track_id`) REFERENCES `track` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1647 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '库存表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock
-- ----------------------------
INSERT INTO `stock` VALUES (131, 1, 4, 180, 78, '2021-03-07 10:48:03', 19, 0, 0, 1, 6, 10654, 0);
INSERT INTO `stock` VALUES (456, 15, 4, 180, 60, '2021-03-10 02:32:16', 5, 1, 4, 1, 3, 9282, 1312);
INSERT INTO `stock` VALUES (777, 23, 5, 250, 86, '2021-03-12 14:55:53', 2, 0, 24, 2, 0, 5390, 4451);
INSERT INTO `stock` VALUES (783, 26, 5, 200, 105, '2021-03-12 15:24:09', 0, 0, 26, 2, 4, 4852, 0);
INSERT INTO `stock` VALUES (1044, 22, 3, 120, 67, '2021-03-14 16:38:20', 0, 0, 5, 1, 3, 10365, 5710);
INSERT INTO `stock` VALUES (1046, 22, 3, 150, 67, '2021-03-14 16:50:18', 1, 1, 5, 1, 3, 10136, 5512);
INSERT INTO `stock` VALUES (1051, 22, 3, 150, 67, '2021-03-14 17:37:57', 2, 1, 5, 1, 3, 9905, 5272);
INSERT INTO `stock` VALUES (1053, 22, 3, 150, 67, '2021-03-14 17:50:10', 3, 1, 5, 1, 3, 9673, 5030);
INSERT INTO `stock` VALUES (1056, 22, 3, 150, 67, '2021-03-14 18:10:59', 4, 1, 6, 1, 3, 9441, 4794);
INSERT INTO `stock` VALUES (1058, 22, 3, 150, 67, '2021-03-14 18:25:33', 5, 1, 6, 1, 3, 9212, 4557);
INSERT INTO `stock` VALUES (1061, 22, 3, 150, 67, '2021-03-14 18:31:46', 6, 1, 5, 1, 3, 8981, 4326);
INSERT INTO `stock` VALUES (1062, 22, 3, 150, 67, '2021-03-14 18:37:29', 7, 1, 6, 1, 3, 8749, 4089);
INSERT INTO `stock` VALUES (1067, 22, 3, 150, 67, '2021-03-14 18:52:21', 8, 1, 5, 1, 3, 8519, 3857);
INSERT INTO `stock` VALUES (1068, 22, 3, 150, 67, '2021-03-14 19:06:14', 9, 1, 5, 1, 3, 8288, 3625);
INSERT INTO `stock` VALUES (1071, 22, 3, 150, 67, '2021-03-14 19:16:47', 10, 1, 5, 1, 3, 8055, 3391);
INSERT INTO `stock` VALUES (1073, 22, 3, 150, 67, '2021-03-14 19:28:18', 11, 1, 5, 1, 3, 7826, 3160);
INSERT INTO `stock` VALUES (1075, 22, 3, 150, 67, '2021-03-14 19:43:20', 12, 1, 6, 1, 3, 7592, 2926);
INSERT INTO `stock` VALUES (1076, 22, 3, 150, 67, '2021-03-14 19:51:59', 13, 1, 5, 1, 3, 7360, 2693);
INSERT INTO `stock` VALUES (1081, 22, 3, 150, 67, '2021-03-14 20:00:28', 14, 1, 6, 1, 3, 7129, 2460);
INSERT INTO `stock` VALUES (1083, 22, 3, 150, 67, '2021-03-14 20:12:13', 15, 1, 6, 1, 3, 6898, 2225);
INSERT INTO `stock` VALUES (1085, 22, 3, 150, 67, '2021-03-14 20:23:47', 16, 1, 6, 1, 3, 6668, 1996);
INSERT INTO `stock` VALUES (1087, 22, 3, 150, 67, '2021-03-14 20:36:52', 17, 1, 6, 1, 3, 6438, 1763);
INSERT INTO `stock` VALUES (1089, 22, 3, 150, 67, '2021-03-14 20:48:44', 18, 2, 6, 1, 3, 6207, 1531);
INSERT INTO `stock` VALUES (1091, 22, 3, 150, 68, '2021-03-14 21:01:34', 0, 0, 6, 1, 3, 10365, 5710);
INSERT INTO `stock` VALUES (1093, 22, 3, 150, 68, '2021-03-14 21:13:19', 1, 1, 6, 1, 3, 10138, 5512);
INSERT INTO `stock` VALUES (1095, 22, 3, 150, 68, '2021-03-14 21:25:10', 2, 1, 6, 1, 3, 9911, 5282);
INSERT INTO `stock` VALUES (1097, 22, 3, 150, 68, '2021-03-14 21:38:27', 3, 1, 6, 1, 3, 9682, 5052);
INSERT INTO `stock` VALUES (1098, 22, 3, 150, 68, '2021-03-14 21:44:40', 4, 1, 5, 1, 3, 9452, 4820);
INSERT INTO `stock` VALUES (1104, 22, 3, 150, 68, '2021-03-14 22:40:39', 5, 1, 6, 1, 3, 9222, 4590);
INSERT INTO `stock` VALUES (1105, 22, 3, 150, 68, '2021-03-14 22:53:39', 6, 1, 6, 1, 3, 8994, 4358);
INSERT INTO `stock` VALUES (1108, 22, 3, 150, 68, '2021-03-14 23:10:13', 7, 1, 6, 1, 3, 8766, 4126);
INSERT INTO `stock` VALUES (1111, 22, 3, 150, 68, '2021-03-14 23:21:53', 8, 1, 6, 1, 3, 8536, 3895);
INSERT INTO `stock` VALUES (1113, 22, 3, 150, 68, '2021-03-14 23:33:39', 9, 1, 6, 1, 3, 8307, 3663);
INSERT INTO `stock` VALUES (1114, 22, 3, 150, 68, '2021-03-14 23:44:44', 10, 1, 6, 1, 3, 8078, 3432);
INSERT INTO `stock` VALUES (1116, 22, 3, 150, 68, '2021-03-14 23:56:38', 11, 1, 6, 1, 3, 7850, 3200);
INSERT INTO `stock` VALUES (1127, 22, 3, 150, 68, '2021-03-15 01:42:09', 12, 1, 6, 1, 3, 7620, 2970);
INSERT INTO `stock` VALUES (1129, 22, 3, 150, 68, '2021-03-15 01:53:33', 13, 1, 6, 1, 3, 7391, 2741);
INSERT INTO `stock` VALUES (1131, 22, 3, 150, 68, '2021-03-15 02:05:05', 14, 1, 6, 1, 3, 7162, 2508);
INSERT INTO `stock` VALUES (1133, 22, 3, 150, 68, '2021-03-15 02:18:34', 15, 1, 6, 1, 3, 6933, 2275);
INSERT INTO `stock` VALUES (1135, 22, 3, 150, 68, '2021-03-15 02:33:10', 16, 1, 6, 1, 3, 6706, 2044);
INSERT INTO `stock` VALUES (1137, 22, 3, 150, 68, '2021-03-15 02:45:45', 17, 1, 6, 1, 3, 6476, 1811);
INSERT INTO `stock` VALUES (1139, 22, 3, 150, 68, '2021-03-15 03:01:16', 18, 1, 6, 1, 3, 6246, 1582);
INSERT INTO `stock` VALUES (1142, 22, 3, 150, 68, '2021-03-15 03:20:21', 19, 2, 6, 1, 3, 6017, 1350);
INSERT INTO `stock` VALUES (1144, 22, 3, 150, 39, '2021-03-15 03:33:00', 0, 0, 6, 1, 2, 5708, 5710);
INSERT INTO `stock` VALUES (1147, 22, 3, 150, 39, '2021-03-15 03:45:55', 1, 1, 6, 1, 2, 5480, 5512);
INSERT INTO `stock` VALUES (1149, 22, 3, 150, 39, '2021-03-15 04:00:43', 2, 1, 6, 1, 2, 5248, 5284);
INSERT INTO `stock` VALUES (1170, 22, 3, 150, 39, '2021-03-15 04:32:36', 3, 1, 5, 1, 2, 5019, 5052);
INSERT INTO `stock` VALUES (1171, 22, 3, 150, 39, '2021-03-15 04:39:10', 4, 1, 6, 1, 2, 4785, 4823);
INSERT INTO `stock` VALUES (1173, 22, 3, 150, 39, '2021-03-15 04:45:44', 5, 1, 5, 1, 2, 4553, 4589);
INSERT INTO `stock` VALUES (1175, 22, 3, 150, 39, '2021-03-15 04:51:39', 6, 1, 6, 1, 2, 4323, 4357);
INSERT INTO `stock` VALUES (1177, 22, 3, 150, 39, '2021-03-15 05:01:37', 7, 1, 6, 1, 2, 4092, 4127);
INSERT INTO `stock` VALUES (1179, 22, 3, 150, 39, '2021-03-15 05:12:39', 8, 1, 6, 1, 2, 3863, 3896);
INSERT INTO `stock` VALUES (1181, 22, 3, 150, 39, '2021-03-15 05:23:54', 9, 1, 6, 1, 2, 3630, 3667);
INSERT INTO `stock` VALUES (1183, 22, 3, 150, 39, '2021-03-15 05:35:24', 10, 1, 6, 1, 2, 3398, 3434);
INSERT INTO `stock` VALUES (1185, 22, 3, 150, 39, '2021-03-15 05:47:45', 11, 1, 6, 1, 2, 3166, 3202);
INSERT INTO `stock` VALUES (1186, 22, 3, 150, 39, '2021-03-15 06:00:10', 12, 1, 6, 1, 2, 2936, 2970);
INSERT INTO `stock` VALUES (1190, 22, 3, 150, 39, '2021-03-15 06:14:39', 13, 1, 6, 1, 2, 2704, 2740);
INSERT INTO `stock` VALUES (1192, 22, 3, 150, 39, '2021-03-15 06:29:40', 14, 1, 6, 1, 2, 2473, 2508);
INSERT INTO `stock` VALUES (1194, 22, 3, 150, 39, '2021-03-15 06:43:11', 15, 1, 6, 1, 2, 2245, 2277);
INSERT INTO `stock` VALUES (1196, 22, 3, 150, 39, '2021-03-15 06:56:09', 16, 1, 6, 1, 2, 2011, 2049);
INSERT INTO `stock` VALUES (1198, 22, 3, 150, 39, '2021-03-15 07:08:40', 17, 1, 6, 1, 2, 1782, 1815);
INSERT INTO `stock` VALUES (1200, 22, 3, 150, 39, '2021-03-15 07:20:52', 18, 1, 6, 1, 2, 1549, 1586);
INSERT INTO `stock` VALUES (1202, 22, 3, 150, 39, '2021-03-15 07:34:15', 19, 2, 6, 1, 2, 1318, 1353);
INSERT INTO `stock` VALUES (1204, 22, 3, 150, 40, '2021-03-15 07:45:47', 0, 0, 6, 1, 2, 5708, 5710);
INSERT INTO `stock` VALUES (1206, 22, 3, 150, 40, '2021-03-15 08:00:18', 1, 1, 6, 1, 2, 5479, 5512);
INSERT INTO `stock` VALUES (1208, 22, 3, 150, 40, '2021-03-15 08:11:54', 2, 1, 6, 1, 2, 5247, 5283);
INSERT INTO `stock` VALUES (1211, 22, 3, 150, 40, '2021-03-15 08:28:11', 3, 1, 6, 1, 2, 5017, 5051);
INSERT INTO `stock` VALUES (1213, 22, 3, 150, 40, '2021-03-15 08:38:40', 4, 1, 6, 1, 2, 4783, 4821);
INSERT INTO `stock` VALUES (1216, 22, 3, 150, 40, '2021-03-15 09:09:32', 5, 1, 6, 1, 2, 4553, 4587);
INSERT INTO `stock` VALUES (1219, 22, 3, 150, 40, '2021-03-15 09:24:51', 6, 1, 6, 1, 2, 4322, 4357);
INSERT INTO `stock` VALUES (1220, 22, 3, 150, 40, '2021-03-15 09:33:54', 7, 1, 6, 1, 2, 4091, 4126);
INSERT INTO `stock` VALUES (1224, 22, 3, 150, 40, '2021-03-15 09:46:48', 8, 1, 6, 1, 2, 3859, 3895);
INSERT INTO `stock` VALUES (1226, 22, 3, 150, 40, '2021-03-15 09:55:53', 9, 1, 6, 1, 2, 3629, 3663);
INSERT INTO `stock` VALUES (1230, 22, 3, 150, 40, '2021-03-15 10:04:41', 10, 1, 6, 1, 2, 3398, 3433);
INSERT INTO `stock` VALUES (1232, 22, 3, 150, 40, '2021-03-15 10:11:53', 11, 1, 6, 1, 2, 3166, 3202);
INSERT INTO `stock` VALUES (1234, 22, 3, 150, 40, '2021-03-15 10:17:10', 12, 1, 5, 1, 2, 2933, 2970);
INSERT INTO `stock` VALUES (1236, 22, 3, 150, 40, '2021-03-15 10:27:22', 13, 1, 6, 1, 2, 2701, 2737);
INSERT INTO `stock` VALUES (1238, 22, 3, 150, 40, '2021-03-15 10:41:35', 14, 1, 6, 1, 2, 2469, 2505);
INSERT INTO `stock` VALUES (1239, 22, 3, 150, 40, '2021-03-15 10:53:21', 15, 1, 6, 1, 2, 2239, 2273);
INSERT INTO `stock` VALUES (1241, 22, 3, 150, 40, '2021-03-15 11:03:03', 16, 1, 6, 1, 2, 2006, 2043);
INSERT INTO `stock` VALUES (1243, 22, 3, 150, 40, '2021-03-15 11:13:48', 17, 1, 6, 1, 2, 1777, 1810);
INSERT INTO `stock` VALUES (1245, 22, 3, 150, 40, '2021-03-15 11:26:07', 18, 1, 6, 1, 2, 1544, 1581);
INSERT INTO `stock` VALUES (1247, 22, 3, 150, 40, '2021-03-15 11:37:21', 19, 2, 6, 1, 2, 1313, 1348);
INSERT INTO `stock` VALUES (1249, 22, 3, 150, 69, '2021-03-15 11:48:14', 0, 0, 6, 1, 3, 10138, 5710);
INSERT INTO `stock` VALUES (1262, 22, 3, 150, 70, '2021-03-15 13:03:56', 0, 0, 5, 1, 3, 9228, 1390);
INSERT INTO `stock` VALUES (1272, 19, 4, 800, 34, '2021-03-15 05:46:00', 9, 0, 0, 1, 2, 0, 0);
INSERT INTO `stock` VALUES (1275, 34, 3, 150, 42, '2021-03-15 05:53:00', 0, 0, 0, 1, 2, 5710, 0);
INSERT INTO `stock` VALUES (1278, 34, 3, 150, 42, '2021-03-15 14:11:18', 2, 1, 6, 1, 2, 5477, 5512);
INSERT INTO `stock` VALUES (1280, 34, 3, 150, 41, '2021-03-15 14:13:00', 0, 0, 0, 1, 2, 5710, 0);
INSERT INTO `stock` VALUES (1282, 34, 3, 150, 41, '2021-03-15 14:24:09', 1, 1, 6, 1, 2, 5473, 5514);
INSERT INTO `stock` VALUES (1283, 34, 3, 150, 71, '2021-03-15 06:28:00', 0, 0, 0, 1, 3, 10365, 0);
INSERT INTO `stock` VALUES (1284, 34, 3, 150, 71, '2021-03-15 06:28:00', 1, 1, 0, 1, 3, 10139, 0);
INSERT INTO `stock` VALUES (1285, 34, 3, 150, 71, '2021-03-15 06:32:00', 2, 1, 0, 1, 3, 9911, 0);
INSERT INTO `stock` VALUES (1286, 34, 3, 150, 71, '2021-03-15 06:28:00', 3, 1, 0, 1, 3, 9683, 0);
INSERT INTO `stock` VALUES (1288, 34, 3, 150, 71, '2021-03-15 14:35:43', 4, 1, 6, 1, 3, 9456, 4863);
INSERT INTO `stock` VALUES (1289, 34, 3, 150, 71, '2021-03-15 14:47:06', 5, 1, 6, 1, 3, 9229, 4575);
INSERT INTO `stock` VALUES (1291, 34, 3, 150, 71, '2021-03-15 14:58:46', 6, 1, 6, 1, 3, 9002, 4341);
INSERT INTO `stock` VALUES (1293, 34, 3, 150, 71, '2021-03-15 15:10:09', 7, 1, 6, 1, 3, 8774, 4107);
INSERT INTO `stock` VALUES (1295, 34, 3, 150, 71, '2021-03-15 15:16:12', 8, 1, 5, 1, 3, 8547, 3872);
INSERT INTO `stock` VALUES (1297, 34, 3, 150, 71, '2021-03-15 15:28:48', 9, 1, 6, 1, 3, 8320, 3637);
INSERT INTO `stock` VALUES (1299, 34, 3, 150, 71, '2021-03-15 15:41:49', 10, 1, 6, 1, 3, 8092, 3402);
INSERT INTO `stock` VALUES (1301, 34, 3, 150, 71, '2021-03-15 15:53:11', 11, 1, 6, 1, 3, 7864, 3171);
INSERT INTO `stock` VALUES (1304, 34, 3, 150, 71, '2021-03-15 16:04:59', 12, 1, 6, 1, 3, 7636, 2934);
INSERT INTO `stock` VALUES (1306, 34, 3, 150, 71, '2021-03-15 16:14:23', 13, 1, 5, 1, 3, 7408, 2702);
INSERT INTO `stock` VALUES (1308, 34, 3, 150, 71, '2021-03-15 16:27:17', 14, 1, 6, 1, 3, 7180, 2465);
INSERT INTO `stock` VALUES (1310, 34, 3, 150, 71, '2021-03-15 16:40:02', 15, 1, 6, 1, 3, 6952, 2233);
INSERT INTO `stock` VALUES (1312, 34, 3, 150, 71, '2021-03-15 16:51:27', 16, 1, 6, 1, 3, 6722, 1998);
INSERT INTO `stock` VALUES (1314, 34, 3, 150, 71, '2021-03-15 17:01:52', 17, 1, 6, 1, 3, 6494, 1763);
INSERT INTO `stock` VALUES (1316, 34, 3, 150, 71, '2021-03-15 17:12:06', 18, 2, 6, 1, 3, 6268, 1531);
INSERT INTO `stock` VALUES (1317, 34, 3, 150, 41, '2021-03-15 17:23:01', 2, 1, 6, 1, 2, 5240, 5277);
INSERT INTO `stock` VALUES (1319, 34, 3, 150, 41, '2021-03-15 17:34:54', 3, 1, 6, 1, 2, 5005, 5044);
INSERT INTO `stock` VALUES (1321, 34, 3, 150, 41, '2021-03-15 17:46:32', 4, 1, 6, 1, 2, 4770, 4809);
INSERT INTO `stock` VALUES (1324, 34, 3, 150, 41, '2021-03-15 17:57:38', 5, 1, 6, 1, 2, 4536, 4574);
INSERT INTO `stock` VALUES (1326, 34, 3, 150, 41, '2021-03-15 18:11:03', 6, 1, 6, 1, 2, 4302, 4340);
INSERT INTO `stock` VALUES (1328, 34, 3, 150, 41, '2021-03-15 18:16:55', 7, 1, 5, 1, 2, 4065, 4106);
INSERT INTO `stock` VALUES (1329, 34, 3, 150, 41, '2021-03-15 18:23:30', 8, 1, 6, 1, 2, 3830, 3869);
INSERT INTO `stock` VALUES (1331, 34, 3, 150, 41, '2021-03-15 18:33:14', 9, 1, 6, 1, 2, 3597, 3634);
INSERT INTO `stock` VALUES (1333, 34, 3, 150, 41, '2021-03-15 18:44:32', 10, 1, 6, 1, 2, 3361, 3401);
INSERT INTO `stock` VALUES (1334, 36, 4, 180, 16, '2021-03-15 18:50:16', 14, 0, 4, 1, 0, 10907, 2747);
INSERT INTO `stock` VALUES (1335, 34, 3, 150, 41, '2021-03-15 18:56:52', 11, 1, 6, 1, 2, 3127, 3165);
INSERT INTO `stock` VALUES (1336, 36, 4, 180, 78, '2021-03-15 19:01:43', 15, 0, 4, 1, 6, 0, 2531);
INSERT INTO `stock` VALUES (1337, 34, 3, 150, 41, '2021-03-15 19:12:59', 12, 1, 6, 1, 2, 2892, 2931);
INSERT INTO `stock` VALUES (1338, 36, 4, 180, 63, '2021-03-15 19:13:44', 16, 0, 4, 1, 3, 6888, 2319);
INSERT INTO `stock` VALUES (1339, 36, 4, 180, 63, '2021-03-15 19:21:12', 17, 1, 4, 1, 3, 6673, 2107);
INSERT INTO `stock` VALUES (1340, 34, 3, 150, 41, '2021-03-15 19:32:39', 13, 1, 6, 1, 2, 2659, 2696);
INSERT INTO `stock` VALUES (1341, 34, 3, 150, 41, '2021-03-15 19:59:05', 14, 1, 6, 1, 2, 2423, 2463);
INSERT INTO `stock` VALUES (1342, 34, 3, 150, 41, '2021-03-15 20:11:09', 15, 1, 6, 1, 2, 2189, 2227);
INSERT INTO `stock` VALUES (1343, 34, 3, 150, 41, '2021-03-15 20:21:19', 16, 1, 6, 1, 2, 1954, 1993);
INSERT INTO `stock` VALUES (1344, 34, 3, 150, 41, '2021-03-15 20:30:44', 17, 1, 6, 1, 2, 1720, 1758);
INSERT INTO `stock` VALUES (1345, 36, 4, 180, 63, '2021-03-15 20:37:38', 18, 1, 3, 1, 3, 6458, 1890);
INSERT INTO `stock` VALUES (1346, 34, 3, 150, 41, '2021-03-15 20:39:43', 18, 2, 6, 1, 2, 1486, 1524);
INSERT INTO `stock` VALUES (1347, 36, 4, 180, 63, '2021-03-15 20:49:35', 19, 1, 4, 1, 3, 6245, 1679);
INSERT INTO `stock` VALUES (1348, 34, 3, 150, 42, '2021-03-15 20:52:40', 3, 1, 6, 1, 2, 5244, 5281);
INSERT INTO `stock` VALUES (1349, 36, 4, 180, 63, '2021-03-15 21:00:55', 20, 1, 4, 1, 3, 6030, 1466);
INSERT INTO `stock` VALUES (1350, 34, 3, 150, 42, '2021-03-15 21:01:58', 4, 1, 6, 1, 2, 5009, 5048);
INSERT INTO `stock` VALUES (1351, 36, 4, 180, 64, '2021-03-15 21:13:50', 0, 0, 4, 1, 3, 10365, 5710);
INSERT INTO `stock` VALUES (1352, 34, 3, 150, 42, '2021-03-15 21:21:04', 5, 1, 5, 1, 2, 4774, 4813);
INSERT INTO `stock` VALUES (1353, 36, 4, 180, 64, '2021-03-15 21:25:24', 1, 1, 4, 1, 3, 10149, 5512);
INSERT INTO `stock` VALUES (1354, 34, 3, 150, 42, '2021-03-15 21:27:15', 6, 1, 6, 1, 2, 4540, 4578);
INSERT INTO `stock` VALUES (1355, 34, 3, 150, 42, '2021-03-15 21:33:20', 7, 1, 6, 1, 2, 4307, 4344);
INSERT INTO `stock` VALUES (1356, 36, 4, 180, 64, '2021-03-15 21:36:45', 2, 1, 4, 1, 3, 9932, 5301);
INSERT INTO `stock` VALUES (1357, 34, 3, 150, 42, '2021-03-15 21:42:14', 8, 1, 6, 1, 2, 4072, 4111);
INSERT INTO `stock` VALUES (1358, 36, 4, 180, 64, '2021-03-15 21:48:46', 3, 1, 4, 1, 3, 9715, 5088);
INSERT INTO `stock` VALUES (1359, 34, 3, 150, 42, '2021-03-15 21:52:18', 9, 1, 6, 1, 2, 3838, 3876);
INSERT INTO `stock` VALUES (1360, 34, 3, 150, 42, '2021-03-15 21:57:54', 10, 1, 5, 1, 2, 3605, 3642);
INSERT INTO `stock` VALUES (1361, 34, 3, 150, 42, '2021-03-15 22:02:48', 11, 1, 6, 1, 2, 3369, 3409);
INSERT INTO `stock` VALUES (1362, 36, 4, 180, 64, '2021-03-15 22:05:12', 4, 1, 4, 1, 3, 9500, 4878);
INSERT INTO `stock` VALUES (1363, 34, 3, 150, 42, '2021-03-15 22:12:38', 12, 1, 6, 1, 2, 3136, 3173);
INSERT INTO `stock` VALUES (1364, 36, 4, 180, 64, '2021-03-15 22:18:49', 5, 1, 4, 1, 3, 9284, 4663);
INSERT INTO `stock` VALUES (1365, 34, 3, 150, 42, '2021-03-15 22:23:47', 13, 1, 6, 1, 2, 2901, 2940);
INSERT INTO `stock` VALUES (1366, 36, 4, 180, 64, '2021-03-15 22:30:26', 6, 1, 4, 1, 3, 9067, 4451);
INSERT INTO `stock` VALUES (1367, 34, 3, 150, 42, '2021-03-15 22:35:11', 14, 1, 6, 1, 2, 2668, 2705);
INSERT INTO `stock` VALUES (1368, 36, 4, 180, 64, '2021-03-15 22:41:33', 7, 1, 4, 1, 3, 8854, 4239);
INSERT INTO `stock` VALUES (1369, 34, 3, 150, 42, '2021-03-15 22:46:40', 15, 1, 6, 1, 2, 2432, 2472);
INSERT INTO `stock` VALUES (1370, 34, 3, 150, 42, '2021-03-15 22:50:50', 16, 1, 6, 1, 2, 2200, 2236);
INSERT INTO `stock` VALUES (1371, 36, 4, 180, 64, '2021-03-15 22:53:10', 8, 1, 4, 1, 3, 8638, 4028);
INSERT INTO `stock` VALUES (1372, 34, 3, 150, 42, '2021-03-15 22:54:48', 17, 2, 5, 1, 2, 1965, 2004);
INSERT INTO `stock` VALUES (1373, 37, 3, 150, 43, '2021-03-15 23:02:09', 0, 0, 6, 1, 2, 5708, 5710);
INSERT INTO `stock` VALUES (1374, 36, 4, 180, 64, '2021-03-15 23:04:35', 9, 1, 4, 1, 3, 8422, 3814);
INSERT INTO `stock` VALUES (1375, 37, 3, 150, 43, '2021-03-15 23:13:13', 1, 1, 6, 1, 2, 5476, 5512);
INSERT INTO `stock` VALUES (1376, 36, 4, 180, 64, '2021-03-15 23:15:57', 10, 1, 4, 1, 3, 8205, 3604);
INSERT INTO `stock` VALUES (1377, 37, 3, 150, 43, '2021-03-15 23:24:19', 2, 1, 6, 1, 2, 5241, 5280);
INSERT INTO `stock` VALUES (1378, 36, 4, 180, 64, '2021-03-15 23:27:23', 11, 1, 4, 1, 3, 7991, 3391);
INSERT INTO `stock` VALUES (1379, 37, 3, 150, 43, '2021-03-15 23:35:48', 3, 1, 6, 1, 2, 5007, 5045);
INSERT INTO `stock` VALUES (1380, 36, 4, 180, 64, '2021-03-15 23:45:21', 12, 1, 4, 1, 3, 7775, 3183);
INSERT INTO `stock` VALUES (1381, 37, 3, 150, 43, '2021-03-15 23:46:38', 4, 1, 6, 1, 2, 4773, 4811);
INSERT INTO `stock` VALUES (1382, 36, 4, 180, 64, '2021-03-15 23:56:55', 13, 1, 4, 1, 3, 7558, 2970);
INSERT INTO `stock` VALUES (1383, 37, 3, 150, 43, '2021-03-15 23:57:46', 5, 1, 6, 1, 2, 4539, 4577);
INSERT INTO `stock` VALUES (1384, 36, 4, 180, 64, '2021-03-16 00:08:18', 14, 1, 4, 1, 3, 7342, 2757);
INSERT INTO `stock` VALUES (1385, 37, 3, 150, 43, '2021-03-16 00:08:59', 6, 1, 6, 1, 2, 4304, 4343);
INSERT INTO `stock` VALUES (1386, 36, 4, 180, 64, '2021-03-16 00:19:34', 15, 1, 4, 1, 3, 7127, 2545);
INSERT INTO `stock` VALUES (1387, 37, 3, 150, 43, '2021-03-16 00:20:05', 7, 1, 6, 1, 2, 4071, 4108);
INSERT INTO `stock` VALUES (1388, 36, 4, 180, 64, '2021-03-16 00:30:59', 16, 1, 4, 1, 3, 6912, 2330);
INSERT INTO `stock` VALUES (1389, 37, 3, 150, 43, '2021-03-16 00:31:08', 8, 1, 6, 1, 2, 3835, 3875);
INSERT INTO `stock` VALUES (1390, 37, 3, 150, 43, '2021-03-16 00:42:14', 9, 1, 6, 1, 2, 3603, 3639);
INSERT INTO `stock` VALUES (1391, 36, 4, 180, 64, '2021-03-16 00:42:55', 17, 1, 4, 1, 3, 6696, 2116);
INSERT INTO `stock` VALUES (1392, 37, 3, 150, 43, '2021-03-16 00:53:21', 10, 1, 6, 1, 2, 3367, 3407);
INSERT INTO `stock` VALUES (1393, 36, 4, 180, 64, '2021-03-16 00:54:28', 18, 1, 4, 1, 3, 6477, 1904);
INSERT INTO `stock` VALUES (1394, 37, 3, 150, 43, '2021-03-16 01:04:42', 11, 1, 6, 1, 2, 3134, 3171);
INSERT INTO `stock` VALUES (1395, 36, 4, 180, 64, '2021-03-16 01:05:57', 19, 1, 4, 1, 3, 6261, 1690);
INSERT INTO `stock` VALUES (1396, 37, 3, 150, 43, '2021-03-16 01:15:39', 12, 1, 6, 1, 2, 2900, 2938);
INSERT INTO `stock` VALUES (1397, 36, 4, 180, 64, '2021-03-16 01:17:30', 20, 2, 4, 1, 3, 6044, 1477);
INSERT INTO `stock` VALUES (1398, 37, 3, 150, 43, '2021-03-16 01:26:48', 13, 1, 6, 1, 2, 2666, 2704);
INSERT INTO `stock` VALUES (1399, 36, 4, 180, 61, '2021-03-16 01:28:59', 0, 0, 4, 1, 3, 10360, 5710);
INSERT INTO `stock` VALUES (1400, 37, 3, 150, 43, '2021-03-16 01:37:57', 14, 1, 6, 1, 2, 2431, 2470);
INSERT INTO `stock` VALUES (1401, 36, 4, 180, 61, '2021-03-16 01:40:42', 1, 1, 4, 1, 3, 10147, 5512);
INSERT INTO `stock` VALUES (1402, 37, 3, 150, 43, '2021-03-16 01:49:06', 15, 1, 6, 1, 2, 2199, 2235);
INSERT INTO `stock` VALUES (1403, 36, 4, 180, 61, '2021-03-16 01:54:43', 2, 1, 4, 1, 3, 9928, 5294);
INSERT INTO `stock` VALUES (1404, 37, 3, 150, 43, '2021-03-16 02:00:26', 16, 1, 6, 1, 2, 1963, 2003);
INSERT INTO `stock` VALUES (1405, 37, 3, 150, 43, '2021-03-16 02:07:46', 17, 1, 6, 1, 2, 1729, 1767);
INSERT INTO `stock` VALUES (1406, 36, 4, 180, 61, '2021-03-16 02:08:35', 3, 1, 3, 1, 3, 9710, 5080);
INSERT INTO `stock` VALUES (1407, 36, 4, 180, 61, '2021-03-16 02:16:24', 4, 1, 4, 1, 3, 9497, 4858);
INSERT INTO `stock` VALUES (1408, 37, 3, 150, 43, '2021-03-16 02:18:32', 18, 2, 6, 1, 2, 1496, 1533);
INSERT INTO `stock` VALUES (1409, 36, 4, 180, 61, '2021-03-16 02:25:55', 5, 1, 4, 1, 3, 9280, 4646);
INSERT INTO `stock` VALUES (1410, 37, 3, 150, 72, '2021-03-16 02:29:40', 0, 0, 6, 1, 3, 10365, 5710);
INSERT INTO `stock` VALUES (1411, 36, 4, 180, 61, '2021-03-16 02:37:23', 6, 1, 4, 1, 3, 9064, 4426);
INSERT INTO `stock` VALUES (1412, 37, 3, 150, 72, '2021-03-16 02:41:19', 1, 1, 6, 1, 3, 10140, 5512);
INSERT INTO `stock` VALUES (1413, 36, 4, 180, 61, '2021-03-16 02:49:06', 7, 1, 4, 1, 3, 8847, 4207);
INSERT INTO `stock` VALUES (1414, 37, 3, 150, 72, '2021-03-16 02:52:23', 2, 1, 6, 1, 3, 9912, 5278);
INSERT INTO `stock` VALUES (1415, 36, 4, 180, 61, '2021-03-16 03:00:35', 8, 1, 4, 1, 3, 8630, 3993);
INSERT INTO `stock` VALUES (1416, 37, 3, 150, 72, '2021-03-16 03:03:30', 3, 1, 6, 1, 3, 9685, 5044);
INSERT INTO `stock` VALUES (1417, 36, 4, 180, 61, '2021-03-16 03:13:28', 9, 1, 4, 1, 3, 8415, 3773);
INSERT INTO `stock` VALUES (1418, 37, 3, 150, 72, '2021-03-16 03:14:37', 4, 1, 6, 1, 3, 9457, 4809);
INSERT INTO `stock` VALUES (1419, 37, 3, 150, 72, '2021-03-16 03:25:27', 5, 1, 6, 1, 3, 9229, 4575);
INSERT INTO `stock` VALUES (1420, 36, 4, 180, 61, '2021-03-16 03:26:04', 10, 1, 4, 1, 3, 8197, 3555);
INSERT INTO `stock` VALUES (1421, 37, 3, 150, 72, '2021-03-16 03:36:41', 6, 1, 6, 1, 3, 8999, 4341);
INSERT INTO `stock` VALUES (1422, 37, 3, 150, 72, '2021-03-16 03:47:46', 7, 1, 6, 1, 3, 8770, 4108);
INSERT INTO `stock` VALUES (1423, 36, 4, 180, 61, '2021-03-16 03:49:30', 11, 1, 3, 1, 3, 7978, 3340);
INSERT INTO `stock` VALUES (1424, 36, 4, 180, 61, '2021-03-16 03:55:23', 12, 1, 4, 1, 3, 7763, 3120);
INSERT INTO `stock` VALUES (1425, 37, 3, 150, 72, '2021-03-16 03:59:02', 8, 1, 6, 1, 3, 8541, 3873);
INSERT INTO `stock` VALUES (1426, 36, 4, 180, 61, '2021-03-16 04:01:19', 13, 1, 4, 1, 3, 7547, 2906);
INSERT INTO `stock` VALUES (1427, 37, 3, 150, 72, '2021-03-16 04:10:07', 9, 1, 6, 1, 3, 8312, 3641);
INSERT INTO `stock` VALUES (1428, 36, 4, 180, 61, '2021-03-16 04:12:28', 14, 1, 4, 1, 3, 7328, 2686);
INSERT INTO `stock` VALUES (1429, 37, 3, 150, 72, '2021-03-16 04:21:11', 10, 1, 6, 1, 3, 8083, 3405);
INSERT INTO `stock` VALUES (1430, 36, 4, 180, 61, '2021-03-16 04:23:30', 15, 1, 3, 1, 3, 7111, 2469);
INSERT INTO `stock` VALUES (1431, 37, 3, 150, 72, '2021-03-16 04:32:18', 11, 1, 6, 1, 3, 7856, 3171);
INSERT INTO `stock` VALUES (1432, 36, 4, 180, 61, '2021-03-16 04:34:19', 16, 1, 4, 1, 3, 6897, 2250);
INSERT INTO `stock` VALUES (1433, 37, 3, 150, 72, '2021-03-16 04:43:24', 12, 1, 6, 1, 3, 7628, 2937);
INSERT INTO `stock` VALUES (1434, 36, 4, 180, 61, '2021-03-16 04:44:39', 17, 1, 4, 1, 3, 6679, 2035);
INSERT INTO `stock` VALUES (1435, 36, 4, 180, 61, '2021-03-16 04:50:15', 18, 2, 4, 1, 3, 6462, 1818);
INSERT INTO `stock` VALUES (1436, 37, 3, 150, 72, '2021-03-16 04:54:22', 13, 1, 6, 1, 3, 7402, 2704);
INSERT INTO `stock` VALUES (1437, 37, 3, 150, 72, '2021-03-16 05:05:41', 14, 1, 6, 1, 3, 7174, 2469);
INSERT INTO `stock` VALUES (1438, 38, 4, 180, 60, '2021-03-16 05:05:58', 0, 0, 3, 1, 3, 10362, 5710);
INSERT INTO `stock` VALUES (1439, 37, 3, 150, 72, '2021-03-16 05:16:46', 15, 1, 6, 1, 3, 6946, 2236);
INSERT INTO `stock` VALUES (1440, 38, 4, 180, 60, '2021-03-16 05:19:25', 1, 1, 4, 1, 3, 10149, 5512);
INSERT INTO `stock` VALUES (1441, 37, 3, 150, 72, '2021-03-16 05:28:08', 16, 1, 6, 1, 3, 6717, 2001);
INSERT INTO `stock` VALUES (1442, 38, 4, 180, 60, '2021-03-16 05:30:54', 2, 1, 4, 1, 3, 9933, 5296);
INSERT INTO `stock` VALUES (1443, 37, 3, 150, 72, '2021-03-16 05:39:21', 17, 1, 6, 1, 3, 6488, 1767);
INSERT INTO `stock` VALUES (1444, 38, 4, 180, 60, '2021-03-16 05:42:28', 3, 1, 4, 1, 3, 9717, 5075);
INSERT INTO `stock` VALUES (1445, 37, 3, 150, 72, '2021-03-16 05:50:34', 18, 2, 6, 1, 3, 6261, 1533);
INSERT INTO `stock` VALUES (1446, 38, 4, 180, 60, '2021-03-16 05:54:17', 4, 1, 4, 1, 3, 9499, 4857);
INSERT INTO `stock` VALUES (1447, 37, 3, 150, 73, '2021-03-16 06:01:34', 0, 0, 6, 1, 3, 10365, 5710);
INSERT INTO `stock` VALUES (1448, 38, 4, 180, 60, '2021-03-16 06:11:20', 6, 1, 4, 1, 3, 9063, 0);
INSERT INTO `stock` VALUES (1449, 37, 3, 150, 73, '2021-03-16 06:16:11', 1, 1, 6, 1, 3, 10135, 0);
INSERT INTO `stock` VALUES (1450, 37, 3, 150, 73, '2021-03-16 07:27:19', 2, 1, 6, 1, 3, 9907, 0);
INSERT INTO `stock` VALUES (1451, 38, 4, 180, 60, '2021-03-16 07:27:21', 7, 1, 3, 1, 3, 8844, 3990);
INSERT INTO `stock` VALUES (1452, 37, 3, 150, 73, '2021-03-16 07:35:41', 4, 1, 5, 1, 3, 9452, 4586);
INSERT INTO `stock` VALUES (1453, 37, 3, 150, 73, '2021-03-16 07:36:03', 3, 1, 5, 1, 3, 9680, 0);
INSERT INTO `stock` VALUES (1454, 38, 4, 180, 60, '2021-03-16 08:29:57', 8, 1, 4, 1, 3, 8628, 3770);
INSERT INTO `stock` VALUES (1455, 37, 3, 150, 73, '2021-03-16 08:33:09', 5, 1, 6, 1, 3, 9224, 4355);
INSERT INTO `stock` VALUES (1456, 38, 4, 180, 60, '2021-03-16 08:35:56', 10, 1, 3, 1, 3, 8197, 3337);
INSERT INTO `stock` VALUES (1457, 38, 4, 180, 60, '2021-03-16 08:35:00', 9, 1, 0, 1, 3, 8415, 0);
INSERT INTO `stock` VALUES (1458, 37, 3, 150, 73, '2021-03-16 08:36:00', 6, 1, 0, 1, 3, 8994, 0);
INSERT INTO `stock` VALUES (1459, 37, 3, 150, 73, '2021-03-16 08:38:45', 7, 1, 5, 1, 3, 8766, 3903);
INSERT INTO `stock` VALUES (1460, 38, 4, 180, 60, '2021-03-16 08:41:33', 11, 1, 4, 1, 3, 7984, 3336);
INSERT INTO `stock` VALUES (1461, 37, 3, 150, 73, '2021-03-16 08:44:27', 8, 1, 6, 1, 3, 8538, 3890);
INSERT INTO `stock` VALUES (1462, 38, 4, 180, 60, '2021-03-16 08:47:06', 12, 1, 4, 1, 3, 7766, 3120);
INSERT INTO `stock` VALUES (1463, 37, 3, 150, 73, '2021-03-16 08:49:33', 9, 1, 6, 1, 3, 8310, 3660);
INSERT INTO `stock` VALUES (1464, 38, 4, 180, 60, '2021-03-16 08:58:47', 13, 1, 4, 1, 3, 7549, 2903);
INSERT INTO `stock` VALUES (1465, 37, 3, 150, 73, '2021-03-16 09:00:22', 10, 1, 6, 1, 3, 8080, 3427);
INSERT INTO `stock` VALUES (1466, 38, 4, 180, 60, '2021-03-16 09:11:22', 14, 1, 4, 1, 3, 7331, 2684);
INSERT INTO `stock` VALUES (1467, 37, 3, 150, 73, '2021-03-16 09:11:47', 11, 1, 6, 1, 3, 7852, 3196);
INSERT INTO `stock` VALUES (1468, 38, 4, 180, 60, '2021-03-16 09:22:26', 15, 1, 4, 1, 3, 7114, 2466);
INSERT INTO `stock` VALUES (1469, 37, 3, 150, 73, '2021-03-16 09:22:49', 12, 1, 6, 1, 3, 7623, 2968);
INSERT INTO `stock` VALUES (1470, 37, 3, 150, 73, '2021-03-16 09:33:59', 13, 1, 6, 1, 3, 7395, 2737);
INSERT INTO `stock` VALUES (1471, 38, 4, 180, 60, '2021-03-16 09:35:20', 16, 1, 4, 1, 3, 6897, 2250);
INSERT INTO `stock` VALUES (1472, 37, 3, 150, 73, '2021-03-16 09:45:31', 14, 1, 6, 1, 3, 7166, 2505);
INSERT INTO `stock` VALUES (1473, 38, 4, 180, 60, '2021-03-16 09:47:52', 17, 1, 4, 1, 3, 6675, 2032);
INSERT INTO `stock` VALUES (1474, 38, 4, 180, 60, '2021-03-16 09:59:23', 18, 1, 4, 1, 3, 6456, 1811);
INSERT INTO `stock` VALUES (1475, 37, 3, 150, 73, '2021-03-16 09:59:34', 15, 1, 6, 1, 3, 6937, 2272);
INSERT INTO `stock` VALUES (1476, 37, 3, 150, 73, '2021-03-16 10:09:31', 16, 1, 5, 1, 3, 6708, 2039);
INSERT INTO `stock` VALUES (1477, 38, 4, 180, 60, '2021-03-16 10:11:54', 19, 1, 4, 1, 3, 6239, 1594);
INSERT INTO `stock` VALUES (1478, 37, 3, 150, 73, '2021-03-16 10:13:18', 17, 1, 6, 1, 3, 6479, 1807);
INSERT INTO `stock` VALUES (1479, 37, 3, 150, 73, '2021-03-16 10:19:51', 18, 1, 6, 1, 3, 6249, 1573);
INSERT INTO `stock` VALUES (1480, 38, 4, 180, 60, '2021-03-16 10:24:50', 20, 2, 4, 1, 3, 6024, 1371);
INSERT INTO `stock` VALUES (1481, 38, 4, 180, 35, '2021-03-16 10:28:44', 0, 0, 3, 1, 2, 5708, 5710);
INSERT INTO `stock` VALUES (1482, 37, 3, 150, 73, '2021-03-16 10:31:03', 19, 2, 6, 1, 3, 6020, 1340);
INSERT INTO `stock` VALUES (1483, 38, 4, 180, 35, '2021-03-16 10:38:41', 1, 1, 4, 1, 2, 5493, 5512);
INSERT INTO `stock` VALUES (1484, 37, 3, 150, 44, '2021-03-16 10:52:08', 0, 0, 6, 1, 2, 5708, 5710);
INSERT INTO `stock` VALUES (1485, 38, 4, 180, 35, '2021-03-16 10:53:19', 2, 1, 4, 1, 2, 5272, 5297);
INSERT INTO `stock` VALUES (1486, 38, 4, 180, 35, '2021-03-16 11:05:09', 3, 1, 4, 1, 2, 5052, 5076);
INSERT INTO `stock` VALUES (1487, 37, 3, 150, 44, '2021-03-16 11:08:00', 1, 1, 6, 1, 2, 5477, 5512);
INSERT INTO `stock` VALUES (1488, 38, 4, 180, 35, '2021-03-16 11:16:50', 4, 1, 4, 1, 2, 4829, 4856);
INSERT INTO `stock` VALUES (1489, 37, 3, 150, 44, '2021-03-16 11:19:46', 2, 1, 6, 1, 2, 5244, 5281);
INSERT INTO `stock` VALUES (1490, 37, 3, 150, 44, '2021-03-16 11:31:19', 3, 1, 6, 1, 2, 5012, 5048);
INSERT INTO `stock` VALUES (1491, 38, 4, 180, 35, '2021-03-16 11:33:57', 5, 1, 4, 1, 2, 4607, 4633);
INSERT INTO `stock` VALUES (1492, 37, 3, 150, 44, '2021-03-16 11:37:47', 4, 1, 5, 1, 2, 4777, 4816);
INSERT INTO `stock` VALUES (1493, 38, 4, 180, 35, '2021-03-16 11:44:05', 6, 1, 3, 1, 2, 4385, 4411);
INSERT INTO `stock` VALUES (1494, 37, 3, 150, 44, '2021-03-16 11:44:11', 5, 1, 6, 1, 2, 4546, 4581);
INSERT INTO `stock` VALUES (1495, 38, 4, 180, 35, '2021-03-16 11:50:05', 7, 1, 4, 1, 2, 4162, 4189);
INSERT INTO `stock` VALUES (1496, 37, 3, 150, 44, '2021-03-16 11:52:59', 6, 1, 6, 1, 2, 4314, 4350);
INSERT INTO `stock` VALUES (1497, 37, 3, 150, 44, '2021-03-16 11:59:55', 7, 1, 6, 1, 2, 4080, 4118);
INSERT INTO `stock` VALUES (1498, 38, 4, 180, 35, '2021-03-16 12:04:22', 8, 1, 4, 1, 2, 3942, 3966);
INSERT INTO `stock` VALUES (1499, 37, 3, 150, 44, '2021-03-16 12:11:01', 8, 1, 6, 1, 2, 3849, 3884);
INSERT INTO `stock` VALUES (1500, 38, 4, 180, 35, '2021-03-16 12:15:45', 9, 1, 4, 1, 2, 3720, 3746);
INSERT INTO `stock` VALUES (1501, 37, 3, 150, 44, '2021-03-16 12:22:13', 9, 1, 6, 1, 2, 3616, 3653);
INSERT INTO `stock` VALUES (1502, 38, 4, 180, 35, '2021-03-16 12:23:34', 10, 1, 3, 1, 2, 3497, 3524);
INSERT INTO `stock` VALUES (1503, 37, 3, 150, 44, '2021-03-16 12:27:39', 10, 1, 5, 1, 2, 3382, 3420);
INSERT INTO `stock` VALUES (1504, 38, 4, 180, 35, '2021-03-16 12:32:40', 11, 1, 4, 1, 2, 3277, 3301);
INSERT INTO `stock` VALUES (1505, 39, 3, 150, 45, '2021-03-16 12:33:35', 0, 0, 6, 1, 2, 5708, 5710);
INSERT INTO `stock` VALUES (1506, 38, 4, 180, 35, '2021-03-16 12:44:00', 12, 1, 4, 1, 2, 3053, 3081);
INSERT INTO `stock` VALUES (1507, 39, 3, 150, 45, '2021-03-16 12:44:43', 1, 1, 6, 1, 2, 5475, 5512);
INSERT INTO `stock` VALUES (1508, 39, 3, 150, 45, '2021-03-16 12:55:49', 2, 1, 6, 1, 2, 5245, 5279);
INSERT INTO `stock` VALUES (1509, 38, 4, 180, 35, '2021-03-16 13:02:38', 13, 1, 4, 1, 2, 2830, 2857);
INSERT INTO `stock` VALUES (1510, 39, 3, 150, 45, '2021-03-16 13:07:00', 3, 1, 6, 1, 2, 5011, 5049);
INSERT INTO `stock` VALUES (1511, 38, 4, 180, 35, '2021-03-16 13:14:41', 14, 1, 4, 1, 2, 2609, 2634);
INSERT INTO `stock` VALUES (1512, 39, 3, 150, 45, '2021-03-16 13:19:23', 4, 1, 6, 1, 2, 4778, 4815);
INSERT INTO `stock` VALUES (1513, 38, 4, 180, 35, '2021-03-16 13:26:00', 15, 1, 4, 1, 2, 2386, 2413);
INSERT INTO `stock` VALUES (1514, 39, 3, 150, 45, '2021-03-16 13:30:25', 5, 1, 6, 1, 2, 4545, 4582);
INSERT INTO `stock` VALUES (1515, 38, 4, 180, 35, '2021-03-16 13:30:41', 16, 1, 3, 1, 2, 2165, 2190);
INSERT INTO `stock` VALUES (1516, 38, 4, 180, 35, '2021-03-16 13:38:24', 17, 1, 4, 1, 2, 1947, 1969);
INSERT INTO `stock` VALUES (1517, 39, 3, 150, 45, '2021-03-16 13:42:19', 6, 1, 6, 1, 2, 4313, 4349);
INSERT INTO `stock` VALUES (1518, 38, 4, 180, 35, '2021-03-16 13:49:45', 18, 1, 4, 1, 2, 1725, 1751);
INSERT INTO `stock` VALUES (1519, 39, 3, 150, 45, '2021-03-16 13:53:42', 7, 1, 6, 1, 2, 4080, 4117);
INSERT INTO `stock` VALUES (1520, 38, 4, 180, 35, '2021-03-16 14:02:03', 19, 2, 4, 1, 2, 1504, 1529);
INSERT INTO `stock` VALUES (1521, 39, 3, 150, 45, '2021-03-16 14:04:42', 8, 1, 6, 1, 2, 3848, 3884);
INSERT INTO `stock` VALUES (1522, 38, 4, 180, 76, '2021-03-16 14:14:19', 0, 0, 4, 1, 5, 1281, 1308);
INSERT INTO `stock` VALUES (1523, 39, 3, 150, 45, '2021-03-16 14:16:30', 9, 1, 6, 1, 2, 3615, 3652);
INSERT INTO `stock` VALUES (1524, 38, 4, 180, 32, '2021-03-16 14:26:53', 0, 0, 4, 1, 2, 5708, 5710);
INSERT INTO `stock` VALUES (1525, 39, 3, 150, 45, '2021-03-16 14:38:12', 10, 1, 6, 1, 2, 3383, 3419);
INSERT INTO `stock` VALUES (1526, 38, 4, 180, 32, '2021-03-16 14:42:19', 1, 1, 4, 1, 2, 5488, 5512);
INSERT INTO `stock` VALUES (1527, 39, 3, 150, 45, '2021-03-16 14:43:27', 11, 1, 5, 1, 2, 3151, 3187);
INSERT INTO `stock` VALUES (1528, 38, 4, 180, 32, '2021-03-16 14:49:59', 2, 1, 4, 1, 2, 5265, 5292);
INSERT INTO `stock` VALUES (1529, 39, 3, 150, 45, '2021-03-16 14:50:42', 12, 1, 6, 1, 2, 2916, 2955);
INSERT INTO `stock` VALUES (1530, 32, 4, 180, 33, '2021-03-16 14:51:34', 0, 0, 3, 1, 2, 5708, 5710);
INSERT INTO `stock` VALUES (1531, 39, 3, 150, 45, '2021-03-16 14:58:23', 13, 1, 6, 1, 2, 2684, 2720);
INSERT INTO `stock` VALUES (1532, 38, 4, 180, 32, '2021-03-16 15:00:58', 3, 1, 4, 1, 2, 5042, 5069);
INSERT INTO `stock` VALUES (1533, 37, 3, 150, 44, '2021-03-16 15:02:12', 11, 1, 5, 1, 2, 3166, 3186);
INSERT INTO `stock` VALUES (1534, 39, 3, 150, 45, '2021-03-16 15:10:12', 14, 1, 6, 1, 2, 2450, 2488);
INSERT INTO `stock` VALUES (1535, 38, 4, 180, 32, '2021-03-16 15:12:11', 4, 1, 4, 1, 2, 4821, 4846);
INSERT INTO `stock` VALUES (1536, 32, 4, 180, 33, '2021-03-16 15:16:16', 1, 2, 3, 1, 2, 5490, 5512);
INSERT INTO `stock` VALUES (1537, 39, 3, 150, 45, '2021-03-16 15:21:05', 15, 1, 6, 1, 2, 2219, 2254);
INSERT INTO `stock` VALUES (1538, 38, 4, 180, 32, '2021-03-16 15:24:31', 5, 1, 4, 1, 2, 4601, 4625);
INSERT INTO `stock` VALUES (1539, 37, 3, 150, 44, '2021-03-16 15:28:32', 12, 2, 5, 1, 2, 2935, 2970);
INSERT INTO `stock` VALUES (1540, 39, 3, 150, 45, '2021-03-16 15:32:35', 16, 2, 6, 1, 2, 1986, 2023);
INSERT INTO `stock` VALUES (1541, 38, 4, 180, 32, '2021-03-16 15:36:17', 6, 2, 4, 1, 2, 0, 4405);
INSERT INTO `stock` VALUES (1542, 32, 4, 180, 5, '2021-03-16 15:36:46', 0, 0, 3, 1, 1, 0, 0);
INSERT INTO `stock` VALUES (1563, 31, 4, 180, 66, '2021-03-17 09:18:59', 0, 0, 0, 1, 3, 10370, 0);
INSERT INTO `stock` VALUES (1564, 31, 4, 180, 66, '2021-03-17 09:19:06', 1, 1, 0, 1, 3, 10153, 0);
INSERT INTO `stock` VALUES (1565, 31, 4, 180, 66, '2021-03-17 09:19:10', 2, 1, 0, 1, 3, 9936, 0);
INSERT INTO `stock` VALUES (1566, 31, 4, 180, 66, '2021-03-17 09:19:12', 3, 1, 0, 1, 3, 9719, 0);
INSERT INTO `stock` VALUES (1567, 31, 4, 180, 66, '2021-03-17 09:19:12', 4, 1, 0, 1, 3, 9502, 0);
INSERT INTO `stock` VALUES (1568, 31, 4, 180, 66, '2021-03-17 09:19:12', 5, 1, 0, 1, 3, 9285, 0);
INSERT INTO `stock` VALUES (1569, 31, 4, 180, 66, '2021-03-17 09:19:12', 6, 1, 0, 1, 3, 9068, 0);
INSERT INTO `stock` VALUES (1570, 31, 4, 180, 66, '2021-03-17 09:19:12', 7, 1, 0, 1, 3, 8851, 0);
INSERT INTO `stock` VALUES (1571, 31, 4, 180, 66, '2021-03-17 09:19:12', 8, 1, 0, 1, 3, 8634, 0);
INSERT INTO `stock` VALUES (1572, 31, 4, 180, 66, '2021-03-17 09:19:12', 9, 1, 0, 1, 3, 8417, 0);
INSERT INTO `stock` VALUES (1573, 31, 4, 180, 66, '2021-03-17 09:19:12', 10, 1, 0, 1, 3, 8200, 0);
INSERT INTO `stock` VALUES (1574, 31, 4, 180, 66, '2021-03-17 09:19:12', 11, 1, 0, 1, 3, 7983, 0);
INSERT INTO `stock` VALUES (1575, 31, 4, 180, 66, '2021-03-17 09:19:12', 12, 1, 0, 1, 3, 7766, 0);
INSERT INTO `stock` VALUES (1576, 31, 4, 180, 66, '2021-03-17 09:19:17', 13, 1, 0, 1, 3, 7549, 0);
INSERT INTO `stock` VALUES (1577, 31, 4, 180, 66, '2021-03-17 09:19:17', 14, 1, 0, 1, 3, 7332, 0);
INSERT INTO `stock` VALUES (1578, 31, 4, 180, 66, '2021-03-17 09:19:17', 15, 1, 0, 1, 3, 7115, 0);
INSERT INTO `stock` VALUES (1579, 31, 4, 180, 66, '2021-03-17 09:19:17', 16, 1, 0, 1, 3, 6898, 0);
INSERT INTO `stock` VALUES (1580, 31, 4, 180, 66, '2021-03-17 09:19:17', 17, 1, 0, 1, 3, 6681, 0);
INSERT INTO `stock` VALUES (1581, 31, 4, 180, 66, '2021-03-17 09:19:17', 18, 1, 0, 1, 3, 6464, 0);
INSERT INTO `stock` VALUES (1582, 31, 4, 180, 66, '2021-03-17 09:19:17', 19, 1, 0, 1, 3, 6247, 0);
INSERT INTO `stock` VALUES (1583, 31, 4, 180, 66, '2021-03-17 09:19:24', 20, 2, 0, 1, 3, 6030, 0);
INSERT INTO `stock` VALUES (1584, 34, 3, 150, 22, '2021-03-17 10:00:54', 0, 0, 0, 1, 2, 5710, 0);
INSERT INTO `stock` VALUES (1585, 34, 3, 150, 22, '2021-03-17 10:00:54', 1, 1, 0, 1, 2, 5210, 0);
INSERT INTO `stock` VALUES (1586, 34, 3, 150, 22, '2021-03-17 10:00:54', 2, 1, 0, 1, 2, 4710, 0);
INSERT INTO `stock` VALUES (1587, 34, 3, 150, 22, '2021-03-17 10:00:54', 3, 1, 0, 1, 2, 4210, 0);
INSERT INTO `stock` VALUES (1588, 34, 3, 150, 22, '2021-03-17 10:00:54', 4, 1, 0, 1, 2, 3710, 0);
INSERT INTO `stock` VALUES (1589, 34, 3, 150, 22, '2021-03-17 10:00:54', 5, 1, 0, 1, 2, 3210, 0);
INSERT INTO `stock` VALUES (1590, 34, 3, 150, 22, '2021-03-17 10:00:54', 6, 1, 0, 1, 2, 2710, 0);
INSERT INTO `stock` VALUES (1591, 34, 3, 150, 22, '2021-03-17 10:00:54', 7, 2, 0, 1, 2, 2210, 0);
INSERT INTO `stock` VALUES (1600, 39, 3, 600, 37, '2021-03-18 10:49:23', 0, 0, 0, 1, 2, 5710, 0);
INSERT INTO `stock` VALUES (1601, 39, 3, 600, 37, '2021-03-18 10:49:23', 1, 1, 0, 1, 2, 5210, 0);
INSERT INTO `stock` VALUES (1602, 39, 3, 600, 37, '2021-03-18 10:49:23', 2, 1, 0, 1, 2, 4710, 0);
INSERT INTO `stock` VALUES (1603, 39, 3, 600, 37, '2021-03-18 10:49:23', 3, 1, 0, 1, 2, 4210, 0);
INSERT INTO `stock` VALUES (1604, 39, 3, 600, 37, '2021-03-18 10:49:23', 4, 1, 0, 1, 2, 3710, 0);
INSERT INTO `stock` VALUES (1605, 39, 3, 600, 37, '2021-03-18 10:49:23', 5, 1, 0, 1, 2, 3210, 0);
INSERT INTO `stock` VALUES (1606, 39, 3, 600, 37, '2021-03-18 10:49:23', 6, 1, 0, 1, 2, 2710, 0);
INSERT INTO `stock` VALUES (1607, 39, 3, 600, 37, '2021-03-18 10:49:23', 7, 2, 0, 1, 2, 2210, 0);
INSERT INTO `stock` VALUES (1608, 35, 4, 180, 62, '2021-03-18 11:20:04', 0, 0, 0, 1, 3, 10370, 0);
INSERT INTO `stock` VALUES (1609, 35, 4, 180, 62, '2021-03-18 11:20:04', 1, 1, 0, 1, 3, 10153, 0);
INSERT INTO `stock` VALUES (1610, 35, 4, 180, 62, '2021-03-18 11:20:04', 2, 1, 0, 1, 3, 9936, 0);
INSERT INTO `stock` VALUES (1611, 35, 4, 180, 62, '2021-03-18 11:20:04', 3, 1, 0, 1, 3, 9719, 0);
INSERT INTO `stock` VALUES (1612, 35, 4, 180, 62, '2021-03-18 11:20:04', 4, 1, 0, 1, 3, 9502, 0);
INSERT INTO `stock` VALUES (1613, 35, 4, 180, 62, '2021-03-18 11:20:04', 5, 1, 0, 1, 3, 9285, 0);
INSERT INTO `stock` VALUES (1614, 35, 4, 180, 62, '2021-03-18 11:20:04', 6, 1, 0, 1, 3, 9068, 0);
INSERT INTO `stock` VALUES (1615, 35, 4, 180, 62, '2021-03-18 11:20:04', 7, 1, 0, 1, 3, 8851, 0);
INSERT INTO `stock` VALUES (1616, 35, 4, 180, 62, '2021-03-18 11:20:04', 8, 1, 0, 1, 3, 8634, 0);
INSERT INTO `stock` VALUES (1617, 35, 4, 180, 62, '2021-03-18 11:20:04', 9, 1, 0, 1, 3, 8417, 0);
INSERT INTO `stock` VALUES (1618, 35, 4, 180, 62, '2021-03-18 11:20:04', 10, 1, 0, 1, 3, 8200, 0);
INSERT INTO `stock` VALUES (1619, 35, 4, 180, 62, '2021-03-18 11:20:04', 11, 1, 0, 1, 3, 7983, 0);
INSERT INTO `stock` VALUES (1620, 35, 4, 180, 62, '2021-03-18 11:20:04', 12, 1, 0, 1, 3, 7766, 0);
INSERT INTO `stock` VALUES (1621, 35, 4, 180, 62, '2021-03-18 11:20:04', 13, 1, 0, 1, 3, 7549, 0);
INSERT INTO `stock` VALUES (1622, 35, 4, 180, 62, '2021-03-18 11:20:04', 14, 1, 0, 1, 3, 7332, 0);
INSERT INTO `stock` VALUES (1623, 35, 4, 180, 62, '2021-03-18 11:20:04', 15, 1, 0, 1, 3, 7115, 0);
INSERT INTO `stock` VALUES (1624, 35, 4, 180, 62, '2021-03-18 11:20:04', 16, 1, 0, 1, 3, 6898, 0);
INSERT INTO `stock` VALUES (1625, 35, 4, 180, 62, '2021-03-18 11:20:04', 17, 1, 0, 1, 3, 6681, 0);
INSERT INTO `stock` VALUES (1626, 35, 4, 180, 62, '2021-03-18 11:20:04', 18, 1, 0, 1, 3, 6464, 0);
INSERT INTO `stock` VALUES (1627, 35, 4, 180, 62, '2021-03-18 11:20:04', 19, 2, 0, 1, 3, 6247, 0);
INSERT INTO `stock` VALUES (1628, 26, 5, 200, 105, '2021-03-18 13:25:25', 1, 1, 0, 2, 4, 4635, 0);
INSERT INTO `stock` VALUES (1629, 26, 5, 200, 105, '2021-03-18 13:25:25', 2, 1, 0, 2, 4, 4418, 0);
INSERT INTO `stock` VALUES (1630, 26, 5, 200, 105, '2021-03-18 13:25:25', 3, 1, 0, 2, 4, 4201, 0);
INSERT INTO `stock` VALUES (1631, 26, 5, 200, 105, '2021-03-18 13:25:25', 4, 1, 0, 2, 4, 3984, 0);
INSERT INTO `stock` VALUES (1632, 26, 5, 200, 105, '2021-03-18 13:25:25', 5, 2, 0, 2, 4, 3767, 0);
INSERT INTO `stock` VALUES (1637, 40, 4, 180, 19, '2021-03-19 16:24:50', 0, 0, 0, 1, 2, 5710, 0);
INSERT INTO `stock` VALUES (1638, 40, 4, 180, 19, '2021-03-19 16:24:50', 1, 2, 0, 1, 2, 5493, 0);
INSERT INTO `stock` VALUES (1639, 40, 4, 180, 20, '2021-03-19 16:24:57', 0, 0, 0, 1, 2, 5710, 0);
INSERT INTO `stock` VALUES (1640, 40, 4, 180, 20, '2021-03-19 16:24:57', 1, 2, 0, 1, 2, 5493, 0);
INSERT INTO `stock` VALUES (1641, 40, 4, 180, 31, '2021-03-19 16:35:47', 0, 0, 0, 1, 2, 5710, 0);
INSERT INTO `stock` VALUES (1642, 40, 4, 180, 31, '2021-03-19 16:35:47', 1, 2, 0, 1, 2, 5493, 0);
INSERT INTO `stock` VALUES (1643, 39, 3, 0, 11, '2021-03-19 17:14:12', 0, 0, 6, 1, 1, 0, 0);
INSERT INTO `stock` VALUES (1644, 3, 4, 0, 3, '2021-03-19 17:15:25', 0, 0, 2, 1, 1, 0, 0);
INSERT INTO `stock` VALUES (1645, 40, 4, 0, 1, '2021-03-19 17:30:08', 0, 0, 1, 1, 1, 0, 0);
INSERT INTO `stock` VALUES (1646, 38, 4, 0, 7, '2021-03-19 17:34:52', 0, 0, 4, 1, 1, 0, 0);
INSERT INTO `stock` VALUES (1647, 37, 3, 0, 9, '2021-03-19 17:35:00', 0, 0, 5, 1, 1, 0, 0);

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
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2825 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '库存记录表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 3779 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '任务表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '砖机-轨道-作业表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

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
  `limit_point_up` smallint(5) NULL DEFAULT NULL COMMENT '轨道上砖极限坐标点',
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
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '轨道表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of track
-- ----------------------------
INSERT INTO `track` VALUES (1, '1-01下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 101, 101, 100, 0, 0, 0, NULL, 10180, NULL, NULL, NULL, NULL, NULL, '10100#10180#10199#10182#101#', NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (2, '1-02下砖轨', 1, NULL, 1, 0, 1, 0, 0, 0, 102, 102, 100, 0, 0, 0, NULL, 10280, NULL, NULL, NULL, NULL, NULL, '10200#10280#10299#10282#102#', NULL, NULL, NULL, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (3, '1-03下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 103, 103, 100, 0, 0, 0, NULL, 10380, NULL, NULL, NULL, NULL, NULL, '10300#10380#10399#10382#103#', NULL, NULL, NULL, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (4, '1-04下砖轨', 1, NULL, 1, 0, 1, 0, 0, 0, 104, 104, 100, 0, 0, 0, NULL, 10480, NULL, NULL, NULL, NULL, NULL, '10400#10480#10499#10482#104#', NULL, NULL, NULL, 8, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (5, '1-05下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 105, 105, 100, 0, 0, 0, NULL, 10580, NULL, NULL, NULL, NULL, NULL, '10500#10580#10599#10582#105#', NULL, NULL, NULL, 11, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (6, '1-06下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 106, 106, 100, 0, 0, 0, NULL, 10680, NULL, NULL, NULL, NULL, NULL, '10600#10680#10699#10682#106#', NULL, NULL, NULL, 13, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (7, '1-07下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 107, 107, 100, 0, 0, 0, NULL, 10780, NULL, NULL, NULL, NULL, NULL, '10700#10780#10799#10782#107#', NULL, NULL, NULL, 16, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (8, '1-08下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 108, 108, 100, 0, 0, 0, NULL, 10880, NULL, NULL, NULL, NULL, NULL, '10800#10880#10899#10882#108#', NULL, NULL, NULL, 18, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (9, '1-09下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 109, 109, 100, 0, 0, 0, NULL, 10980, NULL, NULL, NULL, NULL, NULL, '10900#10980#10999#10982#109#', NULL, NULL, NULL, 20, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (10, '1-10下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 110, 110, 100, 0, 0, 0, NULL, 11080, NULL, NULL, NULL, NULL, NULL, '11000#11080#11099#11082#110#', NULL, NULL, NULL, 23, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (11, '1-11下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 111, 111, 100, 0, 0, 0, NULL, 11180, NULL, NULL, NULL, NULL, NULL, '11100#11180#11199#11182#111#', NULL, NULL, NULL, 25, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (12, '1-12下砖轨', 1, NULL, 1, 1, 1, 0, 0, 0, 112, 112, 100, 0, 0, 0, NULL, 11280, NULL, NULL, NULL, NULL, NULL, '11200#11280#11299#11282#112#', NULL, NULL, NULL, 27, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (13, '1-01上砖轨', 1, NULL, 0, 0, 1, 0, 0, 0, 501, 501, 100, 0, 0, 0, NULL, 50110, NULL, NULL, NULL, NULL, NULL, '50108#50100#50110#50199#501#', NULL, NULL, NULL, 107, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (14, '1-02上砖轨', 1, NULL, 0, 0, 1, 0, 0, 0, 502, 502, 100, 0, 0, 0, NULL, 50210, NULL, NULL, NULL, NULL, NULL, '50208#50200#50210#50299#502#', NULL, NULL, NULL, 110, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (15, '1-03上砖轨', 1, NULL, 0, 1, 1, 0, 0, 0, 503, 503, 100, 0, 0, 0, NULL, 50310, NULL, NULL, NULL, NULL, NULL, '50308#50300#50310#50399#503#', NULL, NULL, NULL, 114, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (16, '1-04上砖轨', 1, NULL, 0, 1, 1, 0, 0, 0, 504, 504, 100, 0, 0, 0, NULL, 50410, NULL, NULL, NULL, NULL, NULL, '50408#50400#50410#50499#504#', NULL, NULL, NULL, 116, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (17, '1-05上砖轨', 1, NULL, 0, 0, 1, 0, 0, 0, 505, 505, 100, 0, 0, 0, NULL, 50510, NULL, NULL, NULL, NULL, NULL, '50508#50500#50510#50599#505#', NULL, NULL, NULL, 123, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (18, '1-06上砖轨', 1, NULL, 0, 0, 1, 0, 0, 0, 506, 506, 100, 0, 0, 0, NULL, 50610, NULL, NULL, NULL, NULL, NULL, '50608#50600#50610#50699#506#', NULL, NULL, NULL, 125, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (19, '1-01入轨', 1, 3, 2, 2, 2, 700, 350, 350, 301, 301, 100, 47, 0, 20, NULL, 30102, 30148, NULL, NULL, NULL, NULL, '30100#30102#30104#30106#30144#30146#30148#301#', 5710, 1305, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (20, '1-02入轨', 1, 3, 2, 2, 1, 700, 350, 350, 302, 302, 100, 48, 19, 21, NULL, 30202, 30248, NULL, NULL, NULL, NULL, '30200#30202#30204#30206#30244#30246#30248#302#', 5710, 1305, NULL, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (21, '1-03入轨', 1, 3, 2, 0, 1, 700, 350, 350, 303, 303, 100, 49, 20, 22, NULL, 30302, 30348, NULL, NULL, NULL, NULL, '30300#30302#30304#30306#30344#30346#30348#303#', 5710, 1305, NULL, 3, 40, 1, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (22, '1-04入轨', 1, 3, 2, 2, 1, 700, 350, 350, 304, 304, 100, 50, 21, 23, NULL, 30402, 30448, NULL, NULL, NULL, NULL, '30400#30402#30404#30406#30444#30446#30448#304#', 5710, 1305, NULL, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (23, '1-05入轨', 1, 3, 2, 0, 1, 700, 350, 350, 305, 305, 100, 51, 22, 24, NULL, 30502, 30548, NULL, NULL, NULL, NULL, '30500#30502#30504#30506#30544#30546#30548#305#', 5710, 1305, NULL, 5, 3, 2, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (24, '1-06入轨', 1, 3, 2, 0, 1, 700, 350, 350, 306, 306, 100, 52, 23, 25, NULL, 30602, 30648, NULL, NULL, NULL, NULL, '30600#30602#30604#30606#30644#30646#30648#306#', 5710, 1305, NULL, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (25, '1-07入轨', 1, 3, 2, 0, 1, 700, 350, 350, 307, 307, 100, 53, 24, 26, NULL, 30702, 30748, NULL, NULL, NULL, NULL, '30700#30702#30704#30706#30744#30746#30748#307#', 5710, 1305, NULL, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (26, '1-08入轨', 1, 3, 2, 0, 1, 700, 350, 350, 308, 308, 100, 54, 25, 27, NULL, 30802, 30848, NULL, NULL, NULL, NULL, '30800#30802#30804#30806#30844#30846#30848#308#', 5710, 1305, NULL, 8, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (27, '1-09入轨', 1, 3, 2, 0, 1, 700, 350, 350, 309, 309, 100, 55, 26, 28, NULL, 30902, 30948, NULL, NULL, NULL, NULL, '30900#30902#30904#30906#30944#30946#30948#309#', 5710, 1305, NULL, 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (28, '1-10入轨', 1, 3, 2, 0, 1, 700, 350, 350, 310, 310, 100, 56, 27, 29, NULL, 31002, 31048, NULL, NULL, NULL, NULL, '31000#31002#31004#31006#31044#31046#31048#310#', 5710, 1305, NULL, 10, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (29, '1-11入轨', 1, 3, 2, 0, 1, 700, 350, 350, 311, 311, 100, 57, 28, 30, NULL, 31102, 31148, NULL, NULL, NULL, NULL, '31100#31102#31104#31106#31144#31146#31148#311#', 5710, 1305, NULL, 11, 16, 3, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (30, '1-12入轨', 1, 3, 2, 0, 1, 700, 350, 350, 312, 312, 100, 58, 29, 31, NULL, 31202, 31248, NULL, NULL, NULL, NULL, '31200#31202#31204#31206#31244#31246#31248#312#', 5710, 1305, NULL, 12, 16, 4, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (31, '1-13入轨', 1, 1, 2, 2, 1, 700, 350, 350, 313, 313, 100, 59, 30, 32, NULL, 31302, 31348, NULL, NULL, NULL, NULL, '31300#31302#31304#31306#31344#31346#31348#313#', 5710, 1305, NULL, 13, 35, 4, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (32, '1-14入轨', 1, 1, 2, 2, 1, 700, 350, 350, 314, 314, 100, 60, 31, 33, NULL, 31402, 31448, NULL, NULL, NULL, NULL, '31400#31402#31404#31406#31444#31446#31448#314#', 5710, 1305, NULL, 14, 38, 4, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (33, '1-15入轨', 1, 1, 2, 1, 1, 700, 350, 350, 315, 315, 100, 61, 32, 34, NULL, 31502, 31548, NULL, NULL, NULL, NULL, '31500#31502#31504#31506#31544#31546#31548#315#', 5710, 1305, NULL, 15, 32, 3, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (34, '1-16入轨', 1, 1, 2, 0, 1, 700, 350, 350, 316, 316, 100, 62, 33, 35, NULL, 31602, 31648, NULL, NULL, NULL, NULL, '31600#31602#31604#31606#31644#31646#31648#316#', 5710, 1305, NULL, 16, 19, 4, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (35, '1-17入轨', 1, 1, 2, 2, 1, 700, 350, 350, 317, 317, 100, 63, 34, 36, NULL, 31702, 31748, NULL, NULL, NULL, NULL, '31700#31702#31704#31706#31744#31746#31748#317#', 5710, 1305, NULL, 17, 38, 4, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (36, '1-18入轨', 1, 1, 2, 0, 1, 700, 350, 350, 318, 318, 100, 64, 35, 37, NULL, 31802, 31848, NULL, NULL, NULL, NULL, '31800#31802#31804#31806#31844#31846#31848#318#', 5710, 1305, NULL, 18, 38, 4, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (37, '1-19入轨', 1, 1, 2, 2, 2, 700, 350, 350, 319, 319, 100, 65, 36, 38, NULL, 31902, 31948, NULL, NULL, NULL, NULL, '31900#31902#31904#31906#31944#31946#31948#319#', 5710, 1305, NULL, 19, 1, 3, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (38, '1-20入轨', 1, 2, 2, 0, 1, 700, 350, 350, 320, 320, 100, 66, 37, 39, NULL, 32002, 32048, NULL, NULL, NULL, NULL, '32000#32002#32004#32006#32044#32046#32048#320#', 5710, 1305, NULL, 20, 3, 4, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (39, '1-21入轨', 1, 2, 2, 2, 1, 700, 350, 350, 321, 321, 100, 67, 38, 40, NULL, 32102, 32148, NULL, NULL, NULL, NULL, '32100#32102#32104#32106#32144#32146#32148#321#', 5710, 1305, NULL, 21, 22, 6, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (40, '1-22入轨', 1, 2, 2, 2, 1, 700, 350, 350, 322, 322, 100, 68, 39, 41, NULL, 32202, 32248, NULL, NULL, NULL, NULL, '32200#32202#32204#32206#32244#32246#32248#322#', 5710, 1305, NULL, 22, 22, 6, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (41, '1-23入轨', 1, 2, 2, 2, 1, 700, 350, 350, 323, 323, 100, 69, 40, 42, NULL, 32302, 32348, NULL, NULL, NULL, NULL, '32300#32302#32304#32306#32344#32346#32348#323#', 5710, 1305, NULL, 23, 34, 6, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (42, '1-24入轨', 1, 2, 2, 2, 1, 700, 350, 350, 324, 324, 100, 70, 41, 43, NULL, 32402, 32448, NULL, NULL, NULL, NULL, '32400#32402#32404#32406#32444#32446#32448#324#', 5710, 1305, NULL, 24, 34, 5, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (43, '1-25入轨', 1, 2, 2, 2, 1, 700, 350, 350, 325, 325, 100, 71, 42, 44, NULL, 32502, 32548, NULL, NULL, NULL, NULL, '32500#32502#32504#32506#32544#32546#32548#325#', 5710, 1305, NULL, 25, 37, 6, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (44, '1-26入轨', 1, 2, 2, 1, 1, 700, 350, 350, 326, 326, 100, 72, 43, 45, NULL, 32602, 32648, NULL, NULL, NULL, NULL, '32600#32602#32604#32606#32644#32646#32648#326#', 5710, 1305, NULL, 26, 37, 5, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (45, '1-27入轨', 1, 2, 2, 1, 1, 700, 350, 350, 327, 327, 100, 73, 44, 46, NULL, 32702, 32748, NULL, NULL, NULL, NULL, '32700#32702#32704#32706#32744#32746#32748#327#', 5710, 1305, NULL, 27, 39, 6, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (46, '1-28入轨', 1, 2, 2, 0, 1, 700, 350, 350, 328, 328, 100, 74, 45, 0, NULL, 32802, 32848, NULL, NULL, NULL, NULL, '32800#32802#32804#32806#32844#32846#32848#328#', 5710, 1305, NULL, 28, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (47, '1-01出轨', 1, 3, 3, 0, 2, 700, 350, 350, 301, 301, 100, 19, 0, 48, NULL, 30198, 30198, NULL, NULL, NULL, NULL, '30154#30156#30194#30196#30198#30199#301#', 5941, 1305, 10370, 101, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (48, '1-02出轨', 1, 3, 3, 0, 1, 700, 350, 350, 302, 302, 100, 20, 47, 49, NULL, 30298, 30298, NULL, NULL, NULL, NULL, '30254#30256#30294#30296#30298#30299#302#', 5941, 1305, 10370, 102, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (49, '1-03出轨', 1, 3, 3, 0, 1, 700, 350, 350, 303, 303, 100, 21, 48, 50, NULL, 30398, 30398, NULL, NULL, NULL, NULL, '30354#30356#30394#30396#30398#30399#303#', 5941, 1305, 10370, 103, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (50, '1-04出轨', 1, 3, 3, 0, 0, 700, 350, 350, 304, 304, 100, 22, 49, 51, NULL, 30498, 30498, NULL, NULL, NULL, NULL, '30454#30456#30494#30496#30498#30499#304#', 5941, 1305, 10370, 104, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (51, '1-05出轨', 1, 3, 3, 0, 1, 700, 350, 350, 305, 305, 100, 23, 50, 52, NULL, 30598, 30598, NULL, NULL, NULL, NULL, '30554#30556#30594#30596#30598#30599#305#', 5941, 1305, 10370, 105, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (52, '1-06出轨', 1, 3, 3, 0, 1, 700, 350, 350, 306, 306, 100, 24, 51, 53, NULL, 30698, 30698, NULL, NULL, NULL, NULL, '30654#30656#30694#30696#30698#30699#306#', 5941, 1305, 10370, 106, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (53, '1-07出轨', 1, 3, 3, 0, 1, 700, 350, 350, 307, 307, 100, 25, 52, 54, NULL, 30798, 30798, NULL, NULL, NULL, NULL, '30754#30756#30794#30796#30798#30799#307#', 5941, 1305, 10370, 107, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (54, '1-08出轨', 1, 3, 3, 0, 1, 700, 350, 350, 308, 308, 100, 26, 53, 55, NULL, 30898, 30898, NULL, NULL, NULL, NULL, '30854#30856#30894#30896#30898#30899#308#', 5941, 1305, 10370, 108, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (55, '1-09出轨', 1, 3, 3, 0, 1, 700, 350, 350, 309, 309, 100, 27, 54, 56, NULL, 30998, 30998, NULL, NULL, NULL, NULL, '30954#30956#30994#30996#30998#30999#309#', 5941, 1305, 10370, 109, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (56, '1-10出轨', 1, 3, 3, 0, 1, 700, 350, 350, 310, 310, 100, 28, 55, 57, NULL, 31098, 31098, NULL, NULL, NULL, NULL, '31054#31056#31094#31096#31098#31099#310#', 5941, 1305, 10370, 110, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (57, '1-11出轨', 1, 3, 3, 0, 1, 700, 350, 350, 311, 311, 100, 29, 56, 58, NULL, 31198, 31198, NULL, NULL, NULL, NULL, '31154#31156#31194#31196#31198#31199#311#', 5941, 1305, 10370, 111, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (58, '1-12出轨', 1, 3, 3, 0, 1, 700, 350, 350, 312, 312, 100, 30, 57, 59, NULL, 31298, 31298, NULL, NULL, NULL, NULL, '31254#31256#31294#31296#31298#31299#312#', 5941, 1305, 10370, 112, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (59, '1-13出轨', 1, 1, 3, 0, 1, 700, 350, 350, 313, 313, 100, 31, 58, 60, NULL, 31398, 31398, NULL, NULL, NULL, NULL, '31354#31356#31394#31396#31398#31399#313#', 5941, 1305, 10370, 113, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (60, '1-14出轨', 1, 1, 3, 2, 1, 700, 350, 350, 314, 314, 100, 32, 59, 61, NULL, 31498, 31498, NULL, NULL, NULL, NULL, '31454#31456#31494#31496#31498#31499#314#', 5941, 1305, 10370, 114, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (61, '1-15出轨', 1, 1, 3, 2, 1, 700, 350, 350, 315, 315, 100, 33, 60, 62, NULL, 31598, 31598, NULL, NULL, NULL, NULL, '31554#31556#31594#31596#31598#31599#315#', 5941, 1305, 10370, 115, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (62, '1-16出轨', 1, 1, 3, 1, 1, 700, 350, 350, 316, 316, 100, 34, 61, 63, NULL, 31698, 31698, NULL, NULL, NULL, NULL, '31654#31656#31694#31696#31698#31699#316#', 5941, 1305, 10370, 116, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (63, '1-17出轨', 1, 1, 3, 1, 1, 700, 350, 350, 317, 317, 100, 35, 62, 64, NULL, 31798, 31798, NULL, NULL, NULL, NULL, '31754#31756#31794#31796#31798#31799#317#', 5941, 1305, 10370, 117, 36, 8, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (64, '1-18出轨', 1, 1, 3, 2, 1, 700, 350, 350, 318, 318, 100, 36, 63, 65, NULL, 31898, 31898, NULL, NULL, NULL, NULL, '31854#31856#31894#31896#31898#31899#318#', 5941, 1305, 10370, 118, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (65, '1-19出轨', 1, 1, 3, 0, 2, 700, 350, 350, 319, 319, 100, 37, 64, 66, NULL, 31998, 31998, NULL, NULL, NULL, NULL, '31954#31956#31994#31996#31998#31999#319#', 5941, 1305, 10370, 119, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (66, '1-20出轨', 1, 2, 3, 1, 1, 700, 350, 350, 320, 320, 100, 38, 65, 67, NULL, 32098, 32098, NULL, NULL, NULL, NULL, '32054#32056#32094#32096#32098#32099#320#', 5941, 1305, 10370, 120, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (67, '1-21出轨', 1, 2, 3, 2, 1, 700, 350, 350, 321, 321, 100, 39, 66, 68, NULL, 32198, 32198, NULL, NULL, NULL, NULL, '32154#32156#32194#32196#32198#32199#321#', 5941, 1305, 10370, 121, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (68, '1-22出轨', 1, 2, 3, 2, 1, 700, 350, 350, 322, 322, 100, 40, 67, 69, NULL, 32298, 32298, NULL, NULL, NULL, NULL, '32254#32256#32294#32296#32298#32299#322#', 5941, 1305, 10370, 122, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (69, '1-23出轨', 1, 2, 3, 2, 1, 700, 350, 350, 323, 323, 100, 41, 68, 70, NULL, 32398, 32398, NULL, NULL, NULL, NULL, '32354#32356#32394#32396#32398#32399#323#', 5941, 1305, 10370, 123, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (70, '1-24出轨', 1, 2, 3, 1, 1, 700, 350, 350, 324, 324, 100, 42, 69, 71, NULL, 32498, 32498, NULL, NULL, NULL, NULL, '32454#32456#32494#32496#32498#32499#324#', 5941, 1305, 10370, 124, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (71, '1-25出轨', 1, 2, 3, 2, 1, 700, 350, 350, 325, 325, 100, 43, 70, 72, NULL, 32598, 32598, NULL, NULL, NULL, NULL, '32554#32556#32594#32596#32598#32599#325#', 5941, 1305, 10370, 125, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (72, '1-26出轨', 1, 2, 3, 2, 1, 700, 350, 350, 326, 326, 100, 44, 71, 73, NULL, 32698, 32698, NULL, NULL, NULL, NULL, '32654#32656#32694#32696#32698#32699#326#', 5941, 1305, 10370, 126, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (73, '1-27出轨', 1, 2, 3, 2, 1, 700, 350, 350, 327, 327, 100, 45, 72, 74, NULL, 32798, 32798, NULL, NULL, NULL, NULL, '32754#32756#32794#32796#32798#32799#327#', 5941, 1305, 10370, 127, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (74, '1-28出轨', 1, 2, 3, 0, 1, 700, 350, 350, 328, 328, 100, 46, 73, 0, NULL, 32898, 32898, NULL, NULL, NULL, NULL, '32854#32856#32894#32896#32898#32899#328#', 5941, 1305, 10370, 128, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (75, '1-B1摆轨', 1, NULL, 5, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 20150, 20150, NULL, NULL, NULL, NULL, '20148#20148#20150#20150#20150#20152#201#', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (76, '1-B2摆轨', 1, NULL, 5, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 20250, 20250, NULL, NULL, NULL, NULL, '20248#20248#20250#20250#20250#20252#202#', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (77, '1-B5摆轨', 1, NULL, 6, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 40150, 40150, NULL, NULL, NULL, NULL, '40148#40148#40150#40150#40152#40152#401#', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (78, '1-B6摆轨', 1, NULL, 6, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 40250, 40250, NULL, NULL, NULL, NULL, '40248#40248#40250#40250#40252#40252#402#', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (79, '2-01下砖轨', 2, NULL, 1, 0, 1, 0, 0, 0, 121, 121, 100, 0, 0, 0, NULL, 12180, NULL, NULL, NULL, NULL, NULL, '12100#12180#12199#12182#121#', NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (80, '2-02下砖轨', 2, NULL, 1, 1, 1, 0, 0, 0, 122, 122, 100, 0, 0, 0, NULL, 12280, NULL, NULL, NULL, NULL, NULL, '12200#12280#12299#12282#122#', NULL, NULL, NULL, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (81, '2-03下砖轨', 2, NULL, 1, 1, 1, 0, 0, 0, 123, 123, 100, 0, 0, 0, NULL, 12380, NULL, NULL, NULL, NULL, NULL, '12300#12380#12399#12382#123#', NULL, NULL, NULL, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (82, '2-04下砖轨', 2, NULL, 1, 0, 1, 0, 0, 0, 124, 124, 100, 0, 0, 0, NULL, 12480, NULL, NULL, NULL, NULL, NULL, '12400#12480#12499#12482#124#', NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (83, '2-05下砖轨', 2, NULL, 1, 1, 1, 0, 0, 0, 125, 125, 100, 0, 0, 0, NULL, 12580, NULL, NULL, NULL, NULL, NULL, '12500#12580#12599#12582#125#', NULL, NULL, NULL, 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (84, '2-06下砖轨', 2, NULL, 1, 1, 1, 0, 0, 0, 126, 126, 100, 0, 0, 0, NULL, 12680, NULL, NULL, NULL, NULL, NULL, '12600#12680#12699#12682#126#', NULL, NULL, NULL, 11, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (85, '01_上砖轨', 2, NULL, 0, 1, 1, 0, 0, 0, 511, 511, 100, 0, 0, 0, NULL, 51110, NULL, NULL, NULL, NULL, NULL, '51108#51100#51110#51199#511#', NULL, NULL, NULL, 22, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (86, '02_上砖轨', 2, NULL, 0, 1, 1, 0, 0, 0, 512, 512, 100, 0, 0, 0, NULL, 51210, NULL, NULL, NULL, NULL, NULL, '51208#51200#51210#51299#512#', NULL, NULL, NULL, 23, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (87, '2-01储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 351, 351, 100, 0, 0, 88, NULL, 35102, 35198, NULL, NULL, NULL, NULL, '35100#35102#35104#35106#35194#35196#35198#35199#351#', NULL, 1305, 4846, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (88, '2-02储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 352, 352, 100, 0, 87, 89, NULL, 35202, 35298, NULL, NULL, NULL, NULL, '35200#35202#35204#35206#35294#35296#35298#35299#352#', NULL, 1305, 4846, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (89, '2-03储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 353, 353, 100, 0, 88, 90, NULL, 35302, 35398, NULL, NULL, NULL, NULL, '35300#35302#35304#35306#35394#35396#35398#35399#353#', NULL, 1305, 4846, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (90, '2-04储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 354, 354, 100, 0, 89, 91, NULL, 35402, 35498, NULL, NULL, NULL, NULL, '35400#35402#35404#35406#35494#35496#35498#35499#354#', NULL, 1305, 4846, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (91, '2-05储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 355, 355, 100, 0, 90, 92, NULL, 35502, 35598, NULL, NULL, NULL, NULL, '35500#35502#35504#35506#35594#35596#35598#35599#355#', NULL, 1305, 4846, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (92, '2-06储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 356, 356, 100, 0, 91, 93, NULL, 35602, 35698, NULL, NULL, NULL, NULL, '35600#35602#35604#35606#35694#35696#35698#35699#356#', NULL, 1305, 4846, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (93, '2-07储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 357, 357, 100, 0, 92, 94, NULL, 35702, 35798, NULL, NULL, NULL, NULL, '35700#35702#35704#35706#35794#35796#35798#35799#357#', NULL, 1305, 4846, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (94, '2-08储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 358, 358, 100, 0, 93, 95, NULL, 35802, 35898, NULL, NULL, NULL, NULL, '35800#35802#35804#35806#35894#35896#35898#35899#358#', NULL, 1305, 4846, 8, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (95, '2-09储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 359, 359, 100, 0, 94, 96, NULL, 35902, 35998, NULL, NULL, NULL, NULL, '35900#35902#35904#35906#35994#35996#35998#35999#359#', NULL, 1305, 4846, 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (96, '2-10储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 360, 360, 100, 0, 95, 97, NULL, 36002, 36098, NULL, NULL, NULL, NULL, '36000#36002#36004#36006#36094#36096#36098#36099#360#', NULL, 1305, 4846, 10, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (97, '2-11储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 361, 361, 100, 0, 96, 98, NULL, 36102, 36198, NULL, NULL, NULL, NULL, '36100#36102#36104#36106#36194#36196#36198#36199#361#', NULL, 1305, 4846, 11, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (98, '2-12储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 362, 362, 100, 0, 97, 99, NULL, 36202, 36298, NULL, NULL, NULL, NULL, '36200#36202#36204#36206#36294#36296#36298#36299#362#', NULL, 1305, 4846, 12, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (99, '2-13储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 363, 363, 100, 0, 98, 100, NULL, 36302, 36398, NULL, NULL, NULL, NULL, '36300#36302#36304#36306#36394#36396#36398#36399#363#', NULL, 1305, 4846, 13, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (100, '2-14储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 364, 364, 100, 0, 99, 101, NULL, 36402, 36498, NULL, NULL, NULL, NULL, '36400#36402#36404#36406#36494#36496#36498#36499#364#', NULL, 1305, 4846, 14, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (101, '2-15储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 365, 365, 100, 0, 100, 102, NULL, 36502, 36598, NULL, NULL, NULL, NULL, '36500#36502#36504#36506#36594#36596#36598#36599#365#', NULL, 1305, 4846, 15, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (102, '2-16储砖轨', 2, NULL, 4, 0, 0, 700, 700, 700, 366, 366, 100, 0, 101, 103, NULL, 36602, 36698, NULL, NULL, NULL, NULL, '36600#36602#36604#36606#36694#36696#36698#36699#366#', NULL, 1305, 4846, 16, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (103, '2-17储砖轨', 2, NULL, 4, 0, 1, 700, 700, 700, 367, 367, 100, 0, 102, 104, NULL, 36702, 36798, NULL, NULL, NULL, NULL, '36700#36702#36704#36706#36794#36796#36798#36799#367#', NULL, 1305, 4846, 17, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (104, '2-18储砖轨', 2, NULL, 4, 0, 1, 700, 700, 700, 368, 368, 100, 0, 103, 105, NULL, 36802, 36898, NULL, NULL, NULL, NULL, '36800#36802#36804#36806#36894#36896#36898#36899#368#', NULL, 1305, 4846, 18, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (105, '2-19储砖轨', 2, NULL, 4, 2, 1, 700, 700, 700, 369, 369, 100, 0, 104, 106, NULL, 36902, 36998, NULL, NULL, NULL, NULL, '36900#36902#36904#36906#36994#36996#36998#36999#369#', NULL, 1305, 4846, 19, 26, 26, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (106, '2-20储砖轨', 2, NULL, 4, 0, 1, 700, 700, 700, 370, 370, 100, 0, 105, 107, NULL, 37002, 37098, NULL, NULL, NULL, NULL, '37000#37002#37004#37006#37094#37096#37098#37099#370#', NULL, 1305, 4846, 20, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (107, '2-21储砖轨', 2, NULL, 4, 0, 1, 700, 700, 700, 371, 371, 100, 0, 106, 108, NULL, 37102, 37198, NULL, NULL, NULL, NULL, '37100#37102#37104#37106#37194#37196#37198#37199#371#', NULL, 1305, 4846, 21, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (108, '2-22储砖轨', 2, NULL, 4, 0, 1, 700, 700, 700, 372, 372, 100, 0, 107, 0, NULL, 37202, 37298, NULL, NULL, NULL, NULL, '37200#37202#37204#37206#37294#37296#37298#37299#372#', NULL, 1305, 4846, 22, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (109, '2-B1摆轨', 2, NULL, 5, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 20350, 20350, NULL, NULL, NULL, NULL, '20348#20348#20350#20350#20350#20352#203#', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `track` VALUES (110, '2-B5摆轨', 2, NULL, 6, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, NULL, 40350, 40350, NULL, NULL, NULL, NULL, '40348#40348#40350#40350#40352#40352#403#', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 1625 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '轨道记录表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 2596 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '交管表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of traffic_control
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
  `trans_id` int(10) UNSIGNED NULL DEFAULT NULL,
  `content` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `createtime` datetime(0) NULL DEFAULT NULL COMMENT '报警时间',
  `resolvetime` datetime(0) NULL DEFAULT NULL COMMENT '解决时间',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `w_createtime_idx`(`createtime`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 12612 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '警告表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '菜单表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 105 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '菜单明细表' ROW_FORMAT = Dynamic;

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
INSERT INTO `wcs_menu_dtl` VALUES (88, 0, '设备及轨道', b'0', 15, 2, 3, b'0');
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
) ENGINE = InnoDB AUTO_INCREMENT = 33 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '模块表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '角色表' ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '用户表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of wcs_user
-- ----------------------------
INSERT INTO `wcs_user` VALUES (1, 'guest', 'guest', '一般用户', '一般操作', 1, b'1', b'1');
INSERT INTO `wcs_user` VALUES (2, 'admin', 'admin', '管理员', '', 2, b'1', b'0');
INSERT INTO `wcs_user` VALUES (3, 'supervisor', 'supervisor', '超级管理员', '', 3, b'1', b'0');

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

SET FOREIGN_KEY_CHECKS = 1;
