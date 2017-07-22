<?php

class org_ascrypt_padding_PKCS7 {
	public function __construct(){}
	static $ERROR_VALUE = "Invalid padding value. Got {0}, expected {1}.";
	static function pad($bytes, $size) {
		$c = $bytes->copy();
		$s = $size - _hx_mod($c->length, $size);
		{
			$_g = 0;
			while($_g < $s) {
				$i = $_g++;
				$c[$c->length] = $s;
				unset($i);
			}
		}
		return $c;
	}
	static function unpad($bytes) {
		$c = $bytes->copy();
		$v = null;
		$s = $c[$c->length - 1];
		{
			$_g = 0;
			while($_g < $s) {
				$i = $_g++;
				$v = $c[$c->length - 1];
				$c->pop();
				if($s !== $v) {
					throw new HException(org_ascrypt_padding_PKCS7_0($_g, $bytes, $c, $i, $s, $v));
				}
				unset($i);
			}
		}
		return $c;
	}
	function __toString() { return 'org.ascrypt.padding.PKCS7'; }
}
function org_ascrypt_padding_PKCS7_0(&$_g, &$bytes, &$c, &$i, &$s, &$v) {
	{
		$string = org_ascrypt_padding_PKCS7::$ERROR_VALUE;
		$args = (new _hx_array(array(Std::string($v), Std::string($s))));
		$l = $args->length;
		{
			$_g1 = 0;
			while($_g1 < $l) {
				$i1 = $_g1++;
				$parts = _hx_explode("{" . _hx_string_rec($i1, "") . "}", $string);
				$string = $parts->join($args[$i1]);
				unset($parts,$i1);
			}
		}
		return $string;
	}
}
