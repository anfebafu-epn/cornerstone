<?php

class org_ascrypt_utilities_ECB {
	public function __construct(){}
	static function encrypt($key, $bytes, $size, $encrypt) {
		$r = (new _hx_array(array()));
		$l = $bytes->length;
		$i = 0;
		while($i < $l) {
			$r = $r->concat(call_user_func_array($encrypt, array($key, $bytes->slice($i, $i + $size))));
			$i += $size;
		}
		return $r;
	}
	static function decrypt($key, $bytes, $size, $decrypt) {
		$r = (new _hx_array(array()));
		$l = $bytes->length;
		$i = 0;
		while($i < $l) {
			$r = $r->concat(call_user_func_array($decrypt, array($key, $bytes->slice($i, $i + $size))));
			$i += $size;
		}
		return $r;
	}
	static function core($k, $b, $s, $c) {
		$r = (new _hx_array(array()));
		$l = $b->length;
		$i = 0;
		while($i < $l) {
			$r = $r->concat(call_user_func_array($c, array($k, $b->slice($i, $i + $s))));
			$i += $s;
		}
		return $r;
	}
	function __toString() { return 'org.ascrypt.utilities.ECB'; }
}
