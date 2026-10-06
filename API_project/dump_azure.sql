-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: azure
-- ------------------------------------------------------
-- Server version	9.5.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ 'e47aa09d-afe1-11f0-b2db-025084af02eb:1-3044';

--
-- Table structure for table `brands`
--

DROP TABLE IF EXISTS `brands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `brands` (
  `id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(90) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `title_UNIQUE` (`title`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `brands`
--

LOCK TABLES `brands` WRITE;
/*!40000 ALTER TABLE `brands` DISABLE KEYS */;
INSERT INTO `brands` VALUES (4,'Brauberg Art'),(5,'Pebeo'),(2,'Гамма'),(3,'Малевичъ'),(1,'Невская Палитра');
/*!40000 ALTER TABLE `brands` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categories`
--

DROP TABLE IF EXISTS `categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categories` (
  `id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categories`
--

LOCK TABLES `categories` WRITE;
/*!40000 ALTER TABLE `categories` DISABLE KEYS */;
INSERT INTO `categories` VALUES (1,'Краски и графика'),(2,'Кисти и инструментарий'),(3,'Холсты и бумага'),(4,'Лепка и скульптура'),(5,'Наборы для творчества');
/*!40000 ALTER TABLE `categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order_items`
--

DROP TABLE IF EXISTS `order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_order` int NOT NULL,
  `id_tovar` int NOT NULL,
  `quantity` int NOT NULL,
  `price` double NOT NULL,
  PRIMARY KEY (`id`),
  KEY `order_idx` (`id_order`),
  KEY `tovat_idx` (`id_tovar`),
  CONSTRAINT `order` FOREIGN KEY (`id_order`) REFERENCES `orders` (`id`),
  CONSTRAINT `tovat` FOREIGN KEY (`id_tovar`) REFERENCES `tovars` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_items`
--

LOCK TABLES `order_items` WRITE;
/*!40000 ALTER TABLE `order_items` DISABLE KEYS */;
INSERT INTO `order_items` VALUES (1,1,1,1,2490),(2,1,5,1,750),(3,2,3,1,1850),(4,3,1,1,2490),(5,4,15,1,990),(6,5,8,1,650),(7,5,12,1,680);
/*!40000 ALTER TABLE `order_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order_statuses`
--

DROP TABLE IF EXISTS `order_statuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_statuses` (
  `id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_statuses`
--

LOCK TABLES `order_statuses` WRITE;
/*!40000 ALTER TABLE `order_statuses` DISABLE KEYS */;
INSERT INTO `order_statuses` VALUES (1,'Новый'),(2,'В обработке'),(3,'Передан в доставку'),(4,'Завершен'),(5,'Отменен');
/*!40000 ALTER TABLE `order_statuses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_user` int NOT NULL,
  `order_date` date NOT NULL,
  `delivery_date` date NOT NULL,
  `total_price` double NOT NULL,
  `id_status` int NOT NULL,
  `id_point` int NOT NULL,
  `payment_type` enum('При получении','Онлайн') NOT NULL,
  PRIMARY KEY (`id`),
  KEY `point_idx` (`id_point`),
  KEY `user_idx` (`id_user`),
  KEY `status_idx` (`id_status`),
  CONSTRAINT `point` FOREIGN KEY (`id_point`) REFERENCES `points` (`id`),
  CONSTRAINT `status` FOREIGN KEY (`id_status`) REFERENCES `order_statuses` (`id`),
  CONSTRAINT `user` FOREIGN KEY (`id_user`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,2,'2026-09-20','2026-09-24',3240,4,1,'Онлайн'),(2,3,'2026-09-22','2026-09-26',1850,4,2,'При получении'),(3,4,'2026-09-25','2026-09-29',2490,3,3,'Онлайн'),(4,5,'2026-09-27','2026-10-01',990,2,4,'При получении'),(5,6,'2026-09-28','2026-10-02',1340,1,5,'Онлайн');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `points`
--

DROP TABLE IF EXISTS `points`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `points` (
  `id` int NOT NULL AUTO_INCREMENT,
  `city` varchar(45) NOT NULL,
  `street` varchar(45) NOT NULL,
  `home_number` int NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `points`
--

LOCK TABLES `points` WRITE;
/*!40000 ALTER TABLE `points` DISABLE KEYS */;
INSERT INTO `points` VALUES (1,'Москва','Арбат',15),(2,'Санкт-Петербург','Невский проспект',88),(3,'Казань','Баумана',42),(4,'Новосибирск','Красный проспект',29),(5,'Екатеринбург','Малышева',10);
/*!40000 ALTER TABLE `points` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tovars`
--

DROP TABLE IF EXISTS `tovars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tovars` (
  `id` int NOT NULL AUTO_INCREMENT,
  `article` varchar(16) NOT NULL,
  `title` varchar(45) NOT NULL,
  `price` double NOT NULL,
  `id_category` int NOT NULL,
  `id_brand` int NOT NULL,
  `description` text NOT NULL,
  `quantity` int NOT NULL,
  `status` enum('Активен','Удален') NOT NULL,
  `photo` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `cat_idx` (`id_category`),
  KEY `brand_idx` (`id_brand`),
  CONSTRAINT `brand` FOREIGN KEY (`id_brand`) REFERENCES `brands` (`id`),
  CONSTRAINT `cat` FOREIGN KEY (`id_category`) REFERENCES `categories` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tovars`
--

LOCK TABLES `tovars` WRITE;
/*!40000 ALTER TABLE `tovars` DISABLE KEYS */;
INSERT INTO `tovars` VALUES (1,'ART-1001','Акварель \"Белые Ночи\" 24 цв.',2490,1,1,'Профессиональные акварельные краски в пластиковой кювете.',45,'Активен','aquarelle_24.jpg'),(2,'ART-1002','Гуашь \"Мастер-Класс\" 12 цв.',890,1,1,'Плотная художественная гуашь с насыщенным пигментом.',60,'Активен','gouache_12.jpg'),(3,'ART-1003','Масло \"Студия\" Набор 10 цв.',1850,1,2,'Масляные краски высшего качества для работы в студии.',25,'Активен','oil_paint.jpg'),(4,'ART-1004','Акрил художественный Белила 75мл',320,1,3,'Густой художественный акрил, светостойкий.',120,'Активен','acrylic_white.jpg'),(5,'ART-1005','Набор кистей Белка №2, 4, 6',750,2,1,'Круглые кисти из натурального ворса белки.',50,'Активен','brushes_squirrel.jpg'),(6,'ART-1006','Набор кистей Синтетика 5шт',490,2,2,'Универсальные плоские и круглые синтетические кисти.',85,'Активен','brushes_synth.jpg'),(7,'ART-1007','Мастихин каплевидный №3',290,2,3,'Нержавеющая сталь с эргономичной деревянной ручкой.',70,'Активен','palette_knife.jpg'),(8,'ART-1008','Холст на подрамнике 30x40 см',650,3,4,'100% хлопок, мелкозернистый, трёхслойный акриловый грунт.',40,'Активен','canvas_30x40.jpg'),(9,'ART-1009','Холст на картоне 20x30 см',280,3,2,'Плотная основа из прессованного картона.',95,'Активен','canvas_board.jpg'),(10,'ART-1010','Альбом для акварели A4 200г/м2',420,3,3,'Склейка для акварели, фактура \"зерно\".',75,'Активен','paper_a4.jpg'),(11,'ART-1011','Полимерная глина \"Craft\" 250g',590,4,4,'Запекаемая глина для создания украшений и миниатюр.',35,'Активен','clay_white.jpg'),(12,'ART-1012','Самозатвердевающая паста 500g',680,4,2,'Масса для скульптуры, застывающая на воздухе.',30,'Активен','air_clay.jpg'),(13,'ART-1013','Стек металлический для лепки 3шт',390,4,3,'Двусторонние инструменты из нержавеющей стали.',50,'Активен','sculpting_tools.jpg'),(14,'ART-1014','Набор для вышивания \"Пейзаж\"',1290,5,2,'В комплекте канва Aida 14, нити мулине и игла.',20,'Активен','embroidery_kit.jpg'),(15,'ART-1015','Картина по номерам \"Париж\"',990,5,4,'Холст 40х50 см, акриловые краски и 3 кисти.',40,'Активен','paint_by_numbers.jpg'),(16,'ART-1016','Алмазная мозаика \"Лев\" 30x40',1150,5,4,'Полная выкладка, квадратные акриловые стразы.',15,'Удален','diamond_painting.jpg'),(17,'ART-1017','Маркеры для скетчинга 36 цв.',1690,1,3,'Двусторонние спиртовые маркеры в текстильном чехле.',45,'Активен','markers_36.jpg'),(18,'ART-1018','Контур по стеклу \"Золото\"',240,1,5,'Объемный акриловый контур для витражной росписи.',60,'Активен','outliner_gold.jpg'),(19,'ART-1019','Эпоксидная смола 300ml',1390,4,5,'Прозрачный двухкомпонентный состав для ювелирных работ.',22,'Активен','epoxy_resin.jpg'),(20,'ART-1020','Деревянный мольберт 120см',2890,3,3,'Напольная конструкция из массива сосны.',12,'Активен','easel_wood.jpg');
/*!40000 ALTER TABLE `tovars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `login` varchar(45) NOT NULL,
  `password` varchar(16) NOT NULL,
  `name` varchar(45) NOT NULL,
  `surname` varchar(45) NOT NULL,
  `patronymic` varchar(45) DEFAULT NULL,
  `email` varchar(45) NOT NULL,
  `status` enum('Активный','Неактивный') NOT NULL,
  `role` enum('Клиент','Администратор') NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin_art','Admin2026!','Иван','Администраторов','Сергеевич','admin@artstore.ru','Активный','Администратор'),(2,'smirnov_draw','pass123','Алексей','Смирнов','Игоревич','a.smirnov@mail.ru','Активный','Клиент'),(3,'masha_paints','pass123','Мария','Кузнецова','Александровна','masha.kuz@gmail.com','Активный','Клиент'),(4,'art_popov','pass123','Артем','Попов','Сергеевич','popov_art@yandex.ru','Активный','Клиент'),(5,'dmitry_v','pass123','Дмитрий','Васильев','Викторович','dmitry.v@mail.ru','Активный','Клиент'),(6,'elena_craft','pass123','Елена','Петрова','Андреевна','elena.petrova@gmail.com','Активный','Клиент'),(7,'sokolov_sketch','pass123','Егор','Соколов','Алексеевич','egor_sok@mail.ru','Активный','Клиент'),(8,'mikhailov_n','pass123','Никита','Михайлов','Дмитриевич','nik.mikhailov@yandex.ru','Активный','Клиент'),(9,'anna_novikova','pass123','Анна','Новикова','Романовна','a.novikova@gmail.com','Активный','Клиент'),(10,'fedorov_ilya','pass123','Илья','Федоров','Евгеньевич','fedorov_i@mail.ru','Активный','Клиент'),(11,'pavel_morozov','pass123','Павел','Морозов','Максимович','p.morozov@yandex.ru','Неактивный','Клиент'),(12,'volkova_olga','pass123','Ольга','Волкова','Сергеевна','volkova_o@gmail.com','Активный','Клиент'),(13,'daria_alex','pass123','Дарья','Алексеева','Игоревна','d.alekseeva@mail.ru','Активный','Клиент'),(14,'tatiana_leb','pass123','Татьяна','Лебедева','Владимировна','leb_tatiana@yandex.ru','Активный','Клиент'),(15,'sveta_semenova','pass123','Светлана','Семенова','Николаевна','semenova_s@gmail.com','Активный','Клиент'),(16,'natalia_egor','pass123','Наталья','Егорова','Олеговна','n.egorova@mail.ru','Активный','Клиент'),(17,'vika_pavlova','pass123','Виктория','Павлова','Анатольевна','v.pavlova@yandex.ru','Активный','Клиент'),(18,'katya_kozlova','pass123','Екатерина','Козлова','Васильевна','k.kozlova@gmail.com','Активный','Клиент'),(19,'marina_step','pass123','Марина','Степанова','Юрьевна','m.stepanova@mail.ru','Активный','Клиент'),(20,'alina_nik','pass123','Алина','Николаева','Денисовна','nik_alina@yandex.ru','Активный','Клиент'),(21,'kristina_orl','pass123','Кристина','Орлова','Павловна','k.orlova@gmail.com','Активный','Клиент'),(22,'kirill_andreev','pass123','Кирилл','Андреев','Сергеевич','andreev_k@mail.ru','Активный','Клиент'),(23,'maksim_makarov','pass123','Максим','Макаров','Артемович','m.makarov@yandex.ru','Активный','Клиент'),(24,'roman_nikitin','pass123','Роман','Никитин','Игоревич','r.nikitin@gmail.com','Активный','Клиент'),(25,'denis_zakharov','pass123','Денис','Захаров','Александрович','denis_z@mail.ru','Активный','Клиент'),(26,'evgeny_zaitsev','pass123','Евгений','Зайцев','Дмитриевич','zaitsev_e@yandex.ru','Активный','Клиент'),(27,'sergey_soloviev','pass123','Сергей','Соловьев','Владимирович','s.soloviev@gmail.com','Активный','Клиент'),(28,'vlad_borisov','pass123','Владислав','Борисов','Максимович','borisov_v@mail.ru','Активный','Клиент'),(29,'gleb_yakovlev','pass123','Глеб','Яковлев','Романович','g.yakovlev@yandex.ru','Активный','Клиент'),(30,'timofey_grig','pass123','Тимофей','Григорьев','Сергеевич','t.grigoriev@gmail.com','Активный','Клиент'),(31,'alex_romanov','pass123','Александр','Романов','Олегович','a.romanov@mail.ru','Активный','Клиент'),(32,'alena_vorob','pass123','Алена','Воробьева','Сергеевна','a.vorobieva@yandex.ru','Активный','Клиент'),(33,'yulia_sergeeva','pass123','Юлия','Сергеева','Игоревна','sergeeva_u@gmail.com','Активный','Клиент'),(34,'ksenia_kuz','pass123','Ксения','Кузьмина','Павловна','kuzmina_k@mail.ru','Активный','Клиент'),(35,'nastya_frolova','pass123','Анастасия','Фролова','Алексеевна','a.frolova@yandex.ru','Активный','Клиент'),(36,'yana_alex','pass123','Яна','Александрова','Дмитриевна','y.alexandrova@gmail.com','Активный','Клиент'),(37,'ira_dmitrieva','pass123','Ирина','Дмитриева','Денисовна','ira_dmitrieva@mail.ru','Активный','Клиент'),(38,'valeria_korol','pass123','Валерия','Королева','Андреевна','v.koroleva@yandex.ru','Активный','Клиент'),(39,'polina_guseva','pass123','Полина','Гусева','Романовна','p.guseva@gmail.com','Активный','Клиент'),(40,'nika_kiseleva','pass123','Вероника','Киселева','Максимовна','n.kiseleva@mail.ru','Активный','Клиент'),(41,'zhenya_ilyina','pass123','Евгения','Ильина','Владимировна','e.ilyina@yandex.ru','Неактивный','Клиент'),(42,'artur_maksimov','pass123','Артур','Максимов','Игоревич','a.maksimov@gmail.com','Активный','Клиент'),(43,'vadim_polyakov','pass123','Вадим','Поляков','Сергеевич','v.polyakov@mail.ru','Активный','Клиент'),(44,'svyat_sorokin','pass123','Святослав','Сорокин','Анатольевич','s.sorokin@yandex.ru','Активный','Клиент'),(45,'lev_vinogradov','pass123','Лев','Виноградов','Евгеньевич','lev_vinogradov@gmail.com','Активный','Клиент'),(46,'mark_kovalev','pass123','Марк','Ковалев','Николаевич','m.kovalev@mail.ru','Активный','Клиент'),(47,'matvey_belov','pass123','Матвей','Белов','Павлович','m.belov@yandex.ru','Активный','Клиент'),(48,'zakhar_medv','pass123','Захар','Медведев','Алексеевич','z.medvedev@gmail.com','Активный','Клиент'),(49,'stepan_antonov','pass123','Степан','Антонов','Артемович','s.antonov@mail.ru','Активный','Клиент'),(50,'fedor_tarasov','pass123','Федор','Тарасов','Дмитриевич','f.tarasov@yandex.ru','Активный','Клиент');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'azure'
--
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-10-03 19:19:37
