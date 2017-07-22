<?php

class org_ascrypt_encoding_UTF8 {
	public function __construct(){}
	static function textToBytes($text) {
		$l = strlen($text);
		$c = null;
		$p = 0;
		$b = (new _hx_array(array()));
		{
			$_g = 0;
			while($_g < $l) {
				$i = $_g++;
				$c = _hx_char_code_at($text, $i);
				if($c <= 127) {
					$b[$p] = $c;
					$p++;
				} else {
					if($c <= 2047) {
						$b[$p] = _hx_shift_right($c, 6) | 192;
						$b[$p + 1] = $c & 63 | 128;
						$p += 2;
					} else {
						if($c <= 65535) {
							$b[$p] = _hx_shift_right($c, 12) | 224;
							$b[$p + 1] = _hx_shift_right($c, 6) & 63 | 128;
							$b[$p + 2] = $c & 63 | 128;
							$p += 3;
						} else {
							if($c <= 1114111) {
								$b[$p] = _hx_shift_right($c, 18) | 240;
								$b[$p + 1] = _hx_shift_right($c, 12) & 63 | 128;
								$b[$p + 2] = _hx_shift_right($c, 6) & 63 | 128;
								$b[$p + 3] = $c & 63 | 128;
								$p += 4;
							}
						}
					}
				}
				unset($i);
			}
		}
		return $b;
	}
	static function bytesToText($bytes) {
		$i = 0;
		$l = $bytes->length;
		$c = null;
		$s = "";
		while($i < $l) {
			$c = 0;
			if(($bytes->a[$i] & 128) !== 128) {
				$c = $bytes[$i];
			} else {
				if(($bytes->a[$i] & 240) === 240) {
					$c |= ($bytes->a[$i] & 7) << 18;
					$c |= ($bytes->a[$i + 1] & 63) << 12;
					$c |= ($bytes->a[$i + 2] & 63) << 6;
					$c |= $bytes->a[$i + 3] & 63;
					$i += 3;
				} else {
					if(($bytes->a[$i] & 224) === 224) {
						$c |= ($bytes->a[$i] & 15) << 12;
						$c |= ($bytes->a[$i + 1] & 63) << 6;
						$c |= $bytes->a[$i + 2] & 63;
						$i += 2;
					} else {
						if(($bytes->a[$i] & 192) === 192) {
							$c |= ($bytes->a[$i] & 31) << 6;
							$c |= $bytes->a[$i + 1] & 63;
							$i++;
						}
					}
				}
			}
			$s .= _hx_string_or_null(chr($c));
			$i++;
		}
		return $s;
	}
	function __toString() { return 'org.ascrypt.encoding.UTF8'; }
}
