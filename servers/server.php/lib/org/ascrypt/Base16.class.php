<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class org_ascrypt_Base16 {
	public function __construct(){}
	static function encode($bytes) {
		$l = $bytes->length;
		$v = null;
		$h = (new _hx_array(array()));
		{
			$_g1 = 0;
			$_g = $l;
			while($_g1 < $_g) {
				$_g1 = $_g1 + 1;
				$i = $_g1 - 1;
				$v = strtolower(StringTools::hex($bytes[$i], null));
				if(strlen($v) < 2) {
					$h[$i] = "0" . _hx_string_or_null($v);
				} else {
					$h[$i] = $v;
				}
				unset($i);
			}
		}
		return $h->join("");
	}
	static function decode($text) {
		$i = 0;
		$l = strlen($text);
		$v = null;
		$b = (new _hx_array(array()));
		while($i < $l) {
			$v = _hx_substr($text, $i, 2);
			$b[Std::int($i / 2)] = Std::parseInt("0x" . _hx_string_or_null($v));
			$i = $i + 2;
		}
		return $b;
	}
	function __toString() { return 'org.ascrypt.Base16'; }
}
