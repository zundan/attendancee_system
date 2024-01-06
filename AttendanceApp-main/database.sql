CREATE DATABASE `attendance`;

USE attendance;

CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(255) NOT NULL,
  `nama` varchar(255) NOT NULL,
  `pangkat` varchar(255) NOT NULL,
  `assigned_event` varchar(255),
  `password` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE `attendance` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nama` varchar(255) NOT NULL,
  `event` varchar(255) NOT NULL,
  `date` date NOT NULL DEFAULT CURRENT_DATE,
  `time` time NOT NULL DEFAULT CURRENT_TIME,
  `attendance` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE `events` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `event` varchar(255) NOT NULL,
  `location` varchar(255) NOT NULL,
  `date` date NOT NULL,
  PRIMARY KEY (`id`)
);

INSERT INTO users (email, nama, pangkat, password) VALUES ('admin@gmail.com', 'danda', 'admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918')