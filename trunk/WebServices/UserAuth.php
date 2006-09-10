<?php # Auth.php
require_once("NyFolderDb.php");
require_once("utils.php");

if (Authentication() === true) {
	echo XmlOutput("authentication", "User Authentication Ok");
} else {
	echo XmlOutput("error", "User Authentication Failed");
}
?>
