<?php

class org_ascrypt_Base64 {
	public function __construct(){}
	static $chrs = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
	static function encode($bytes) {
		$l = $bytes->length;
		$c1 = 0;
		$c2 = 0;
		$c3 = 0;
		$e1 = 0;
		$e2 = 0;
		$e3 = 0;
		$e4 = 0;
		$i = 0;
		$t = "";
		while($i < $l) {
			$c1 = $bytes[$i++];
			$c2 = $bytes[$i++];
			$c3 = $bytes[$i++];
			$e1 = $c1 >> 2;
			$e2 = ($c1 & 3) << 4 | $c2 >> 4;
			$e3 = ($c2 & 15) << 2 | $c3 >> 6;
			$e4 = $c3 & 63;
			$t .= _hx_string_or_null(_hx_char_at(org_ascrypt_Base64::$chrs, $e1)) . _hx_string_or_null(_hx_char_at(org_ascrypt_Base64::$chrs, $e2));
			if($i <= $l) {
				$t .= _hx_string_or_null(_hx_char_at(org_ascrypt_Base64::$chrs, $e3));
			}
			if($i <= $l) {
				$t .= _hx_string_or_null(_hx_char_at(org_ascrypt_Base64::$chrs, $e4));
			}
		}
		if($c2 === 0) {
			$t .= "=";
		}
		if($c3 === 0) {
			$t .= "=";
		}
		return $t;
	}
	static function decode($text) {
		$l = strlen($text);
		$c1 = 0;
		$c2 = 0;
		$c3 = 0;
		$e1 = 0;
		$e2 = 0;
		$e3 = 0;
		$e4 = 0;
		$i = 0;
		$b = (new _hx_array(array()));
		while($i < $l) {
			$e1 = _hx_index_of(org_ascrypt_Base64::$chrs, _hx_char_at($text, $i++), null);
			$e2 = _hx_index_of(org_ascrypt_Base64::$chrs, _hx_char_at($text, $i++), null);
			$e3 = _hx_index_of(org_ascrypt_Base64::$chrs, _hx_char_at($text, $i++), null);
			$e4 = _hx_index_of(org_ascrypt_Base64::$chrs, _hx_char_at($text, $i++), null);
			$c1 = $e1 << 2 | $e2 >> 4;
			$c2 = ($e2 & 15) << 4 | $e3 >> 2;
			$c3 = ($e3 & 3) << 6 | $e4;
			$b->push($c1);
			if($e3 !== 64) {
				$b->push($c2);
			}
			if($e4 !== 64) {
				$b->push($c3);
			}
		}
		return $b;
	}
	function __toString() { return 'org.ascrypt.Base64'; }
}
