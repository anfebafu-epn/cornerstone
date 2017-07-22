<?php

class org_ascrypt_utilities_CTR {
	public function __construct(){}
	static function encrypt($key, $bytes, $size, $encrypt, $iv) {
		$bl = $bytes->length;
		$e = (new _hx_array(array()));
		$x = $iv->copy();
		$i = 0;
		while($i < $bl) {
			$e = call_user_func_array($encrypt, array($key, $x));
			{
				$_g = 0;
				while($_g < $size) {
					$j = $_g++;
					$bytes->a[$i + $j] ^= $e[$j];
					unset($j);
				}
				unset($_g);
			}
			$l = $size - 1;
			while($l >= 0) {
				--$l;
				$x->a[$l]++;
				if($x[$l] !== 0) {
					break;
				}
			}
			$i += $size;
			unset($l);
		}
		return $bytes;
	}
	static function decrypt($key, $bytes, $size, $encrypt, $iv) {
		$bl = $bytes->length;
		$e = (new _hx_array(array()));
		$x = $iv->copy();
		$i = 0;
		while($i < $bl) {
			$e = call_user_func_array($encrypt, array($key, $x));
			{
				$_g = 0;
				while($_g < $size) {
					$j = $_g++;
					$bytes->a[$i + $j] ^= $e[$j];
					unset($j);
				}
				unset($_g);
			}
			$l = $size - 1;
			while($l >= 0) {
				--$l;
				$x->a[$l]++;
				if($x[$l] !== 0) {
					break;
				}
			}
			$i += $size;
			unset($l);
		}
		return $bytes;
	}
	static function core($k, $b, $s, $c, $v) {
		$bl = $b->length;
		$e = (new _hx_array(array()));
		$x = $v->copy();
		$i = 0;
		while($i < $bl) {
			$e = call_user_func_array($c, array($k, $x));
			{
				$_g = 0;
				while($_g < $s) {
					$j = $_g++;
					$b->a[$i + $j] ^= $e[$j];
					unset($j);
				}
				unset($_g);
			}
			$l = $s - 1;
			while($l >= 0) {
				--$l;
				$x->a[$l]++;
				if($x[$l] !== 0) {
					break;
				}
			}
			$i += $s;
			unset($l);
		}
		return $b;
	}
	function __toString() { return 'org.ascrypt.utilities.CTR'; }
}
