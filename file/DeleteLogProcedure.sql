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
	delete from consume_log where DATEDIFF(CURRENT_DATE,consume_time) >= 31;
	/*仅保留 7 天的交管数据*/
	delete from traffic_control where DATEDIFF(CURRENT_DATE,create_time) >= 7;
	/*仅保留 7 天的砖机需求数据*/
	delete from tilelifterneed where finish = 1 and DATEDIFF(CURRENT_DATE,create_time) >= 7;
END
;;
delimiter ;

-- ----------------------------
-- Event structure for DELETE_DATA_EVENT
-- ----------------------------
DROP EVENT IF EXISTS `DELETE_DATA_EVENT`;
delimiter ;;
CREATE DEFINER = `root`@`localhost` EVENT `DELETE_DATA_EVENT`
ON SCHEDULE
EVERY '1' DAY STARTS '2020-08-13 01:00:00'
COMMENT '每日一次删除过期无效的历史数据'
DO CALL DELETE_DATA()
;;
delimiter ;
