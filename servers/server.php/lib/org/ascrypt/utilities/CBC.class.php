<?php

class org_ascrypt_utilities_CBC {
	public function __construct(){}
	static function encrypt($key, $bytes, $size, $encrypt, $iv) {
		$r = (new _hx_array(array()));
		$l = $bytes->length;
		$i = 0;
		while($i < $l) {
			{
				$_g = 0;
				while($_g < $size) {
					$j = $_g++;
					$bytes->a[$i + $j] ^= $iv[$j];
					unset($j);
				}
				unset($_g);
			}
			$r = $r->concat(call_user_func_array($encrypt, array($key, $bytes->slice($i, $i + $size))));
			$iv = $r->slice($i, $i + $size);
			$i += $size;
		}
		return $r;
	}
	static function decrypt($key, $bytes, $size, $decrypt, $iv) {
		$l = $bytes->length;
		$t = null;
		$r = (new _hx_array(array()));
		$i = 0;
		while($i < $l) {
			$t = $bytes->slice($i, $i + $size);
			$r = $r->concat(call_user_func_array($decrypt, array($key, $t)));
			{
				$_g = 0;
				while($_g < $size) {
					$j = $_g++;
					$r->a[$i + $j] ^= $iv[$j];
					unset($j);
				}
				unset($_g);
			}
			$iv = $t->slice(0, $size);
			$i += $size;
		}
		return $r;
	}
	function __toString() { return 'org.ascrypt.utilities.CBC'; }
}
