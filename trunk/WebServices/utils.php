<?php
require_once("NyFolderDb.php");

function VarRequest ($name, $description) {
	if (!isset($_REQUEST[$name])) {
		echo XmlOutput("error", $description . " Variable not Set.");
		exit;
	}
	return($_REQUEST[$name]);
}

function GetUserMagic ($username) {
	$db = new NyFolderDb;
	if ($db->Initialize() === true) {
		$magic = $db->GetMagic($username);
		$db->Destroy();
		return($magic);
	} else {
		echo XmlOutput("error", "Database Connection Failed");
		exit;
	}
}

function Authentication() {
	# Get UserName & User Magic
	$userName = VarRequest('user', 'UserName');	
	$userMixMagic = VarRequest('magic', 'Magic');

	# Generate Correct Magic
	$myIp = RealIP();
	$userMagic = GetUserMagic($userName);
	$realMagic = md5(sha1($myIp) . sha1($userMagic));

	return(($realMagic == $userMixMagic) ? true : false);
}

# Quote variable to make safe
function SqlQuote ($value) {
	# Stripslashes
	if (get_magic_quotes_gpc())
		$value = stripslashes($value);
	# Quote if not a number or a numeric string
	if (!is_numeric($value))
		$value = "'" . mysql_real_escape_string($value) . "'";
	return $value;
}

function XmlQuote ($value) {
	# Stripslashes
	if (get_magic_quotes_gpc())
		$value = stripslashes($value);
	$value = "'" . $value . "'";
	return $value;
}

function XmlOutput ($firstTag, $bodyText, $attributes=false) {
	$attributes = ($attributes === false) ? "" : " " . $attributes;
	return("<{$firstTag}${attributes}>".base64_encode($bodyText)."</{$firstTag}>");
}

# Returns the real IP address of the user
function RealIP() {
	# No IP found (will be overwritten by for
	# if any IP is found behind a firewall)
	# $ip = FALSE;
  
	# If HTTP_CLIENT_IP is set, then give it priority
	if (!empty($_SERVER["HTTP_CLIENT_IP"]))
		$ip = $_SERVER["HTTP_CLIENT_IP"];
  
	# User is behind a proxy and check that we discard RFC1918 IP addresses
	# if they are behind a proxy then only figure out which IP belongs to the
	# user.  Might not need any more hackin if there is a squid reverse proxy
	# infront of apache.
	if (!empty($_SERVER['HTTP_X_FORWARDED_FOR'])) {
		# Put the IP's intUnsaved Document 1o an array which we shall work with shortly.
		$ips = explode (", ", $_SERVER['HTTP_X_FORWARDED_FOR']);
		if ($ip) {array_unshift($ips, $ip); $ip = FALSE;}

		for ($i = 0; $i < count($ips); $i++) {
			# Skip RFC 1918 IP's 10.0.0.0/8, 172.16.0.0/12 and
			# 192.168.0.0/16 -- jim kill me later with my regexp pattern
			# below.
			if (!eregi ("^(10|172\.16|192\.168)\.", $ips[$i])) {
				if (version_compare(phpversion(), "5.0.0", ">=")) {
					if (ip2long($ips[$i]) != false) {
						$ip = $ips[$i];
						break;
					}
				} else {
					if (ip2long($ips[$i]) != -1) {
						$ip = $ips[$i];
						break;
					}
				}
			}
		}
	}
	# Return with the found IP or the remote address
	return($ip ? $ip : $_SERVER['REMOTE_ADDR']);
}
?>
