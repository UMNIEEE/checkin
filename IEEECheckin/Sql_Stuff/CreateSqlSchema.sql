CREATE DATABASE  IF NOT EXISTS `ieee` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `ieee`;
-- MySQL dump 10.13  Distrib 5.6.17, for Win32 (x86)
--
-- Host: localhost    Database: ieee
-- ------------------------------------------------------
-- Server version	5.6.19-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `ent_event`
--

DROP TABLE IF EXISTS `ent_event`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ent_event` (
  `EventId` int(11) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `Type` int(11) NOT NULL,
  `StartDate` datetime NOT NULL,
  `EndDate` datetime NOT NULL,
  `Hosts` varchar(500) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `BuildingName` varchar(150) DEFAULT NULL,
  `BuildingRooms` varchar(120) DEFAULT NULL,
  `StreetAddress` varchar(500) DEFAULT NULL,
  `City` varchar(150) DEFAULT NULL,
  `ZipCode` varchar(10) DEFAULT NULL,
  `StateOrProvince` varchar(100) DEFAULT NULL,
  `Country` varchar(150) DEFAULT NULL,
  `DateCreated` datetime NOT NULL,
  `DateLastModified` datetime NOT NULL,
  PRIMARY KEY (`EventId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ent_event`
--

LOCK TABLES `ent_event` WRITE;
/*!40000 ALTER TABLE `ent_event` DISABLE KEYS */;
/*!40000 ALTER TABLE `ent_event` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ent_person`
--

DROP TABLE IF EXISTS `ent_person`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ent_person` (
  `PersonId` int(11) NOT NULL,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(200) NOT NULL,
  `PhoneNumberOne` varchar(20) DEFAULT NULL,
  `PhoneNumberTwo` varchar(20) DEFAULT NULL,
  `EmailOne` varchar(150) DEFAULT NULL,
  `EmailTwo` varchar(150) DEFAULT NULL,
  `ContactMethodEnum` int(11) DEFAULT NULL,
  `ContactMethodOther` varchar(500) DEFAULT NULL,
  `Facebook` varchar(500) DEFAULT NULL,
  `Twitter` varchar(150) DEFAULT NULL,
  `ShirtSize` int(11) DEFAULT NULL,
  `StreetAddress` varchar(500) DEFAULT NULL,
  `AptNumber` varchar(20) DEFAULT NULL,
  `City` varchar(150) DEFAULT NULL,
  `ZipCode` varchar(10) DEFAULT NULL,
  `StateOrProvince` varchar(100) DEFAULT NULL,
  `Country` varchar(150) DEFAULT NULL,
  `UCollege` int(11) DEFAULT NULL,
  `UX500` varchar(15) DEFAULT NULL,
  `UStudentId` int(11) DEFAULT NULL,
  `UEmail` varchar(150) DEFAULT NULL,
  `UCampus` int(11) DEFAULT NULL,
  `ULatestEnrollSem` int(11) DEFAULT NULL,
  `ULatestEnrollYear` int(11) DEFAULT NULL,
  `UGradSem` int(11) DEFAULT NULL,
  `UGradYear` int(11) DEFAULT NULL,
  `UMajors` varchar(500) DEFAULT NULL,
  `UMinors` varchar(500) DEFAULT NULL,
  `DateCreated` datetime NOT NULL,
  `DateLastModified` datetime NOT NULL,
  PRIMARY KEY (`PersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ent_person`
--

LOCK TABLES `ent_person` WRITE;
/*!40000 ALTER TABLE `ent_person` DISABLE KEYS */;
/*!40000 ALTER TABLE `ent_person` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eventparticipants`
--

DROP TABLE IF EXISTS `eventparticipants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `eventparticipants` (
  `EventParticipantId` int(11) NOT NULL,
  `EventId` int(11) NOT NULL,
  `PersonId` int(11) NOT NULL,
  `Role` int(11) DEFAULT NULL,
  `IsPayedFee` bit(1) DEFAULT NULL,
  `DateCreated` datetime NOT NULL,
  `DateLastModified` datetime NOT NULL,
  PRIMARY KEY (`EventParticipantId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eventparticipants`
--

LOCK TABLES `eventparticipants` WRITE;
/*!40000 ALTER TABLE `eventparticipants` DISABLE KEYS */;
/*!40000 ALTER TABLE `eventparticipants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `members`
--

DROP TABLE IF EXISTS `members`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `members` (
  `MemberId` int(11) NOT NULL,
  `PersonId` int(11) NOT NULL,
  `IEEEId` int(11) NOT NULL,
  `YearOfMembership` int(11) NOT NULL,
  `Role` int(11) NOT NULL,
  `RoleOthers` varchar(200) DEFAULT NULL,
  `SubCommittees` varchar(500) DEFAULT NULL,
  `Other` varchar(500) DEFAULT NULL,
  `DateCreated` datetime NOT NULL,
  `DateLastModified` datetime NOT NULL,
  PRIMARY KEY (`MemberId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `members`
--

LOCK TABLES `members` WRITE;
/*!40000 ALTER TABLE `members` DISABLE KEYS */;
/*!40000 ALTER TABLE `members` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_campus`
--

DROP TABLE IF EXISTS `type_campus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_campus` (
  `CampusId` int(11) NOT NULL,
  `Campus` varchar(100) NOT NULL,
  `CampusCode` varchar(50) NOT NULL,
  PRIMARY KEY (`CampusId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_campus`
--

LOCK TABLES `type_campus` WRITE;
/*!40000 ALTER TABLE `type_campus` DISABLE KEYS */;
INSERT INTO `type_campus` VALUES (1,'Crookston','c'),(2,'Duluth','d'),(3,'Morris','m'),(4,'Rochester','r'),(5,'Twin Cities','t'),(6,'Other','o');
/*!40000 ALTER TABLE `type_campus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_college`
--

DROP TABLE IF EXISTS `type_college`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_college` (
  `CollegeId` int(11) NOT NULL,
  `College` varchar(500) NOT NULL,
  `CollegeAbriv` varchar(10) DEFAULT NULL,
  `School` varchar(500) NOT NULL,
  `SchoolAbriv` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`CollegeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_college`
--

LOCK TABLES `type_college` WRITE;
/*!40000 ALTER TABLE `type_college` DISABLE KEYS */;
INSERT INTO `type_college` VALUES (1,'University of Minnesota - Twin Cities','UofM-TC','College of Science and Engineering','CSE'),(2,'University of Minnesota - Twin Cities','UofM-TC','College of Liberal Arts','CLA'),(3,'University of Minnesota - Twin Cities','UofM-TC','College of Biological Sciences','CBS'),(4,'University of Minnesota - Twin Cities','UofM-TC','Carlson School of Management','CSOM'),(5,'University of Minnesota - Twin Cities','UofM-TC','College of Design','CDES'),(6,'University of Minnesota - Twin Cities','UofM-TC','College of Food, Agriculture and Natural Resource Sciences','CFANS'),(7,'University of Minnesota - Twin Cities','UofM-TC','College of Education and Human Development','CEHD'),(8,'University of Minnesota - Twin Cities','UofM-TC','Graduate School','Grad'),(9,'University of Minnesota - Twin Cities','UofM-TC','Other','Other');
/*!40000 ALTER TABLE `type_college` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_contactmethod`
--

DROP TABLE IF EXISTS `type_contactmethod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_contactmethod` (
  `ContactMethodId` int(11) NOT NULL,
  `ContactMethod` varchar(100) NOT NULL,
  PRIMARY KEY (`ContactMethodId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_contactmethod`
--

LOCK TABLES `type_contactmethod` WRITE;
/*!40000 ALTER TABLE `type_contactmethod` DISABLE KEYS */;
INSERT INTO `type_contactmethod` VALUES (1,'Email'),(2,'Facebook'),(4,'Twitter'),(8,'Text'),(16,'Call'),(32,'Mail'),(64,'Other');
/*!40000 ALTER TABLE `type_contactmethod` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_country`
--

DROP TABLE IF EXISTS `type_country`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_country` (
  `CountryId` int(11) NOT NULL,
  `Country` varchar(150) NOT NULL,
  `Abbreviation` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`CountryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_country`
--

LOCK TABLES `type_country` WRITE;
/*!40000 ALTER TABLE `type_country` DISABLE KEYS */;
INSERT INTO `type_country` VALUES (1,'USA','USA'),(2,'Canada','CA'),(3,'Mexico','MEX');
/*!40000 ALTER TABLE `type_country` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_enrollsem`
--

DROP TABLE IF EXISTS `type_enrollsem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_enrollsem` (
  `EnrollSemId` int(11) NOT NULL,
  `EnrollSem` varchar(50) NOT NULL,
  PRIMARY KEY (`EnrollSemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_enrollsem`
--

LOCK TABLES `type_enrollsem` WRITE;
/*!40000 ALTER TABLE `type_enrollsem` DISABLE KEYS */;
INSERT INTO `type_enrollsem` VALUES (1,'Fall'),(2,'Spring'),(3,'May Term'),(4,'Summer');
/*!40000 ALTER TABLE `type_enrollsem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_eventtype`
--

DROP TABLE IF EXISTS `type_eventtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_eventtype` (
  `EventTypeId` int(11) NOT NULL,
  `EventType` varchar(150) NOT NULL,
  PRIMARY KEY (`EventTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_eventtype`
--

LOCK TABLES `type_eventtype` WRITE;
/*!40000 ALTER TABLE `type_eventtype` DISABLE KEYS */;
INSERT INTO `type_eventtype` VALUES (1,'Recruitment'),(2,'General Meeting'),(3,'Collaborative'),(4,'Committee'),(5,'Officer'),(6,'Other');
/*!40000 ALTER TABLE `type_eventtype` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_role`
--

DROP TABLE IF EXISTS `type_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_role` (
  `RoleId` int(11) NOT NULL,
  `Role` varchar(100) NOT NULL,
  PRIMARY KEY (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_role`
--

LOCK TABLES `type_role` WRITE;
/*!40000 ALTER TABLE `type_role` DISABLE KEYS */;
INSERT INTO `type_role` VALUES (1,'Member'),(2,'Officer'),(3,'Committee'),(4,'Associate'),(5,'Participant'),(6,'Volunteer'),(7,'Presenter'),(8,'Advisor'),(9,'Other');
/*!40000 ALTER TABLE `type_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_shirtsize`
--

DROP TABLE IF EXISTS `type_shirtsize`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_shirtsize` (
  `ShirtSizeId` int(11) NOT NULL,
  `ShirtSize` varchar(10) NOT NULL,
  PRIMARY KEY (`ShirtSizeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_shirtsize`
--

LOCK TABLES `type_shirtsize` WRITE;
/*!40000 ALTER TABLE `type_shirtsize` DISABLE KEYS */;
INSERT INTO `type_shirtsize` VALUES (1,'YXS'),(2,'YS'),(3,'YM'),(4,'YL'),(5,'S'),(6,'M'),(7,'L'),(8,'XL'),(9,'XL2'),(10,'XL3'),(11,'XL4');
/*!40000 ALTER TABLE `type_shirtsize` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `type_stateprovince`
--

DROP TABLE IF EXISTS `type_stateprovince`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `type_stateprovince` (
  `StateProvinceId` int(11) NOT NULL,
  `StateProvince` varchar(100) NOT NULL,
  `Abbreviation` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`StateProvinceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `type_stateprovince`
--

LOCK TABLES `type_stateprovince` WRITE;
/*!40000 ALTER TABLE `type_stateprovince` DISABLE KEYS */;
INSERT INTO `type_stateprovince` VALUES (1,'Alabama','AL'),(2,'Alaska','AK'),(3,'Arizona','AZ'),(4,'Arkansas','AR'),(5,'California','CA'),(6,'Colorado','CO'),(7,'Connecticut','CT'),(8,'Delaware','DE'),(9,'Florida','FL'),(10,'Georgia','GA'),(11,'Hawaii','HI'),(12,'Idaho','ID'),(13,'Illinois','IL'),(14,'Indiana','IN'),(15,'Iowa','IW'),(16,'Kansas','KS'),(17,'Kentucky','KY'),(18,'Louisiana','LA'),(19,'Maine','ME'),(20,'Maryland','MD'),(21,'Massachusetts','MA'),(22,'Michigan','MI'),(23,'Minnesota','MN'),(24,'Mississippi','MS'),(25,'Missouri','MO'),(26,'Montana','MT'),(27,'Nebraska','NE'),(28,'Nevada','NV'),(29,'New Hampshire','NH'),(30,'New Jersey','NJ'),(31,'New Mexico','NM'),(32,'New York','NY'),(33,'North Carolina','NC'),(34,'North Dakota','ND'),(35,'Ohio','OH'),(36,'Oklahoma','OK'),(37,'Oregon','OR'),(38,'Pennsylvania','PA'),(39,'Rhode Island','RI'),(40,'Sourth Carolina','SC'),(41,'South Dakota','SD'),(42,'Tennessee','TN'),(43,'Texas','TX'),(44,'Utah','UT'),(45,'Vermont','VT'),(46,'Virginia','VA'),(47,'Washington','WA'),(48,'West Virginia','WV'),(49,'Wisconsin','WI'),(50,'Wyoming','WY'),(51,'District of Columbia','DC'),(52,'Alberta',NULL),(53,'British Columbia',NULL),(54,'Manitoba',NULL),(55,'New Brunswick',NULL),(56,'Newfoundland',NULL),(57,'Northwest Territories',NULL),(58,'Novia Scotia',NULL),(59,'Nunavut',NULL),(60,'Ontario',NULL),(61,'Prince Edward Island',NULL),(62,'Quebec',NULL),(63,'Saskatchewan',NULL),(64,'Yukon',NULL),(65,'Other',NULL);
/*!40000 ALTER TABLE `type_stateprovince` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2014-09-17 22:47:33
