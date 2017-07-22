<?php

class org_ascrypt_AES {
	public function __construct(){}
	static $ERROR_KEY = "Invalid key size. Key size needs to be either 128, 192 or 256 bits.\x0A";
	static $ERROR_MODE = "Invalid mode of operation. Supported modes are ECB, CBC, CTR or NONE.\x0A";
	static $ERROR_BLOCK = "Invalid block size. Block size is fixed at 128 bits.\x0A";
	static $xtime;
	static $isbox;
	static $isrtab;
	static $srtab;
	static $sbox;
	static function encrypt($key, $bytes, $mode = null, $iv = null) {
		if($mode === null) {
			$mode = "ecb";
		}
		{
			$kl = $key->length;
			if($kl !== 16 && $kl !== 24 && $kl !== 32) {
				throw new HException(org_ascrypt_AES::$ERROR_KEY);
			}
			if(_hx_mod($bytes->length, 16) !== 0) {
				throw new HException(org_ascrypt_AES::$ERROR_BLOCK);
			}
		}
		$k = $key->copy();
		$b = $bytes->copy();
		{
			org_ascrypt_AES::$isrtab = new _hx_array(array());
			org_ascrypt_AES::$isbox = new _hx_array(array());
			org_ascrypt_AES::$xtime = new _hx_array(array());
			{
				$_g = 0;
				while($_g < 256) {
					$i = $_g++;
					org_ascrypt_AES::$isbox[org_ascrypt_AES::$sbox[$i]] = $i;
					unset($i);
				}
			}
			{
				$_g1 = 0;
				while($_g1 < 16) {
					$j = $_g1++;
					org_ascrypt_AES::$isrtab[org_ascrypt_AES::$srtab[$j]] = $j;
					unset($j);
				}
			}
			{
				$_g2 = 0;
				while($_g2 < 128) {
					$k1 = $_g2++;
					org_ascrypt_AES::$xtime[$k1] = $k1 << 1;
					org_ascrypt_AES::$xtime[128 + $k1] = $k1 << 1 ^ 27;
					unset($k1);
				}
			}
		}
		{
			$kl1 = $k->length;
			$ks = 0;
			$rcon = 1;
			switch($kl1) {
			case 16:{
				$ks = 176;
			}break;
			case 24:{
				$ks = 208;
			}break;
			case 32:{
				$ks = 240;
			}break;
			}
			$i1 = $kl1;
			while($i1 < $ks) {
				$t = $k->slice($i1 - 4, $i1);
				if(_hx_mod($i1, $kl1) === 0) {
					$t = (new _hx_array(array(org_ascrypt_AES::$sbox->a[$t[1]] ^ $rcon, org_ascrypt_AES::$sbox[$t[2]], org_ascrypt_AES::$sbox[$t[3]], org_ascrypt_AES::$sbox[$t[0]])));
					if(($rcon <<= 1) >= 256) {
						$rcon ^= 283;
					}
				} else {
					if($kl1 > 24 && _hx_mod($i1, $kl1) === 16) {
						$t = (new _hx_array(array(org_ascrypt_AES::$sbox[$t[0]], org_ascrypt_AES::$sbox[$t[1]], org_ascrypt_AES::$sbox[$t[2]], org_ascrypt_AES::$sbox[$t[3]])));
					}
				}
				$j1 = 0;
				while($j1 < 4) {
					$k[$i1 + $j1] = $k->a[$i1 + $j1 - $kl1] ^ $t[$j1];
					$j1++;
				}
				$i1 += 4;
				unset($t,$j1);
			}
		}
		{
			$_g3 = strtolower($mode);
			switch($_g3) {
			case "ecb":{
				$encrypt = (isset(org_ascrypt_AES::$ie) ? org_ascrypt_AES::$ie: array("org_ascrypt_AES", "ie"));
				{
					$r = (new _hx_array(array()));
					$l = $b->length;
					$i2 = 0;
					while($i2 < $l) {
						$r = $r->concat(call_user_func_array($encrypt, array($k, $b->slice($i2, $i2 + 16))));
						$i2 += 16;
					}
					return $r;
				}
			}break;
			case "cbc":{
				$encrypt1 = (isset(org_ascrypt_AES::$ie) ? org_ascrypt_AES::$ie: array("org_ascrypt_AES", "ie"));
				$iv1 = $iv->copy();
				$r1 = (new _hx_array(array()));
				$l1 = $b->length;
				$i3 = 0;
				while($i3 < $l1) {
					{
						$_g4 = 0;
						while($_g4 < 16) {
							$j2 = $_g4++;
							$b->a[$i3 + $j2] ^= $iv1[$j2];
							unset($j2);
						}
						unset($_g4);
					}
					$r1 = $r1->concat(call_user_func_array($encrypt1, array($k, $b->slice($i3, $i3 + 16))));
					$iv1 = $r1->slice($i3, $i3 + 16);
					$i3 += 16;
				}
				return $r1;
			}break;
			case "ctr":{
				$encrypt2 = (isset(org_ascrypt_AES::$ie) ? org_ascrypt_AES::$ie: array("org_ascrypt_AES", "ie"));
				$iv2 = $iv->copy();
				{
					$bl = $b->length;
					$e = (new _hx_array(array()));
					$x = $iv2->copy();
					$i4 = 0;
					while($i4 < $bl) {
						$e = call_user_func_array($encrypt2, array($k, $x));
						{
							$_g5 = 0;
							while($_g5 < 16) {
								$j3 = $_g5++;
								$b->a[$i4 + $j3] ^= $e[$j3];
								unset($j3);
							}
							unset($_g5);
						}
						$l2 = 15;
						while($l2 >= 0) {
							--$l2;
							$x->a[$l2]++;
							if($x[$l2] !== 0) {
								break;
							}
						}
						$i4 += 16;
						unset($l2);
					}
					return $b;
				}
			}break;
			case "none":{
				$b1 = $b->copy();
				$i5 = 16;
				$l3 = $k->length;
				{
					$r2 = $k->slice(0, 16);
					{
						$_g6 = 0;
						while($_g6 < 16) {
							$i6 = $_g6++;
							$b1->a[$i6] ^= $r2[$i6];
							unset($i6);
						}
					}
				}
				while($i5 < $l3 - 16) {
					{
						$b2 = org_ascrypt_AES::$sbox;
						{
							$_g7 = 0;
							while($_g7 < 16) {
								$i7 = $_g7++;
								$b1[$i7] = $b2[$b1[$i7]];
								unset($i7);
							}
							unset($_g7);
						}
						unset($b2);
					}
					{
						$t1 = org_ascrypt_AES::$srtab;
						$h = $b1->copy();
						{
							$_g8 = 0;
							while($_g8 < 16) {
								$i8 = $_g8++;
								$b1[$i8] = $h[$t1[$i8]];
								unset($i8);
							}
							unset($_g8);
						}
						unset($t1,$h);
					}
					{
						$i9 = 0;
						while($i9 < 16) {
							$s0 = $b1[$i9];
							$s1 = $b1[$i9 + 1];
							$s2 = $b1[$i9 + 2];
							$s3 = $b1[$i9 + 3];
							$h1 = $s0 ^ $s1 ^ $s2 ^ $s3;
							$b1->a[$i9] ^= $h1 ^ org_ascrypt_AES::$xtime[$s0 ^ $s1];
							$b1->a[$i9 + 1] ^= $h1 ^ org_ascrypt_AES::$xtime[$s1 ^ $s2];
							$b1->a[$i9 + 2] ^= $h1 ^ org_ascrypt_AES::$xtime[$s2 ^ $s3];
							$b1->a[$i9 + 3] ^= $h1 ^ org_ascrypt_AES::$xtime[$s3 ^ $s0];
							$i9 += 4;
							unset($s3,$s2,$s1,$s0,$h1);
						}
						unset($i9);
					}
					{
						$r3 = $k->slice($i5, $i5 + 16);
						{
							$_g9 = 0;
							while($_g9 < 16) {
								$i10 = $_g9++;
								$b1->a[$i10] ^= $r3[$i10];
								unset($i10);
							}
							unset($_g9);
						}
						unset($r3);
					}
					$i5 += 16;
				}
				{
					$b3 = org_ascrypt_AES::$sbox;
					{
						$_g10 = 0;
						while($_g10 < 16) {
							$i11 = $_g10++;
							$b1[$i11] = $b3[$b1[$i11]];
							unset($i11);
						}
					}
				}
				{
					$t2 = org_ascrypt_AES::$srtab;
					$h2 = $b1->copy();
					{
						$_g11 = 0;
						while($_g11 < 16) {
							$i12 = $_g11++;
							$b1[$i12] = $h2[$t2[$i12]];
							unset($i12);
						}
					}
				}
				{
					$r4 = $k->slice($i5, $i5 + 16);
					{
						$_g12 = 0;
						while($_g12 < 16) {
							$i13 = $_g12++;
							$b1->a[$i13] ^= $r4[$i13];
							unset($i13);
						}
					}
				}
				return $b1;
			}break;
			default:{
				throw new HException(org_ascrypt_AES::$ERROR_MODE);
			}break;
			}
		}
	}
	static function decrypt($key, $bytes, $mode = null, $iv = null) {
		if($mode === null) {
			$mode = "ecb";
		}
		{
			$kl = $key->length;
			if($kl !== 16 && $kl !== 24 && $kl !== 32) {
				throw new HException(org_ascrypt_AES::$ERROR_KEY);
			}
			if(_hx_mod($bytes->length, 16) !== 0) {
				throw new HException(org_ascrypt_AES::$ERROR_BLOCK);
			}
		}
		$k = $key->copy();
		$b = $bytes->copy();
		{
			org_ascrypt_AES::$isrtab = new _hx_array(array());
			org_ascrypt_AES::$isbox = new _hx_array(array());
			org_ascrypt_AES::$xtime = new _hx_array(array());
			{
				$_g = 0;
				while($_g < 256) {
					$i = $_g++;
					org_ascrypt_AES::$isbox[org_ascrypt_AES::$sbox[$i]] = $i;
					unset($i);
				}
			}
			{
				$_g1 = 0;
				while($_g1 < 16) {
					$j = $_g1++;
					org_ascrypt_AES::$isrtab[org_ascrypt_AES::$srtab[$j]] = $j;
					unset($j);
				}
			}
			{
				$_g2 = 0;
				while($_g2 < 128) {
					$k1 = $_g2++;
					org_ascrypt_AES::$xtime[$k1] = $k1 << 1;
					org_ascrypt_AES::$xtime[128 + $k1] = $k1 << 1 ^ 27;
					unset($k1);
				}
			}
		}
		{
			$kl1 = $k->length;
			$ks = 0;
			$rcon = 1;
			switch($kl1) {
			case 16:{
				$ks = 176;
			}break;
			case 24:{
				$ks = 208;
			}break;
			case 32:{
				$ks = 240;
			}break;
			}
			$i1 = $kl1;
			while($i1 < $ks) {
				$t = $k->slice($i1 - 4, $i1);
				if(_hx_mod($i1, $kl1) === 0) {
					$t = (new _hx_array(array(org_ascrypt_AES::$sbox->a[$t[1]] ^ $rcon, org_ascrypt_AES::$sbox[$t[2]], org_ascrypt_AES::$sbox[$t[3]], org_ascrypt_AES::$sbox[$t[0]])));
					if(($rcon <<= 1) >= 256) {
						$rcon ^= 283;
					}
				} else {
					if($kl1 > 24 && _hx_mod($i1, $kl1) === 16) {
						$t = (new _hx_array(array(org_ascrypt_AES::$sbox[$t[0]], org_ascrypt_AES::$sbox[$t[1]], org_ascrypt_AES::$sbox[$t[2]], org_ascrypt_AES::$sbox[$t[3]])));
					}
				}
				$j1 = 0;
				while($j1 < 4) {
					$k[$i1 + $j1] = $k->a[$i1 + $j1 - $kl1] ^ $t[$j1];
					$j1++;
				}
				$i1 += 4;
				unset($t,$j1);
			}
		}
		{
			$_g3 = strtolower($mode);
			switch($_g3) {
			case "ecb":{
				$decrypt = (isset(org_ascrypt_AES::$id) ? org_ascrypt_AES::$id: array("org_ascrypt_AES", "id"));
				{
					$r = (new _hx_array(array()));
					$l = $b->length;
					$i2 = 0;
					while($i2 < $l) {
						$r = $r->concat(call_user_func_array($decrypt, array($k, $b->slice($i2, $i2 + 16))));
						$i2 += 16;
					}
					return $r;
				}
			}break;
			case "cbc":{
				$decrypt1 = (isset(org_ascrypt_AES::$id) ? org_ascrypt_AES::$id: array("org_ascrypt_AES", "id"));
				$iv1 = $iv->copy();
				$l1 = $b->length;
				$t1 = null;
				$r1 = (new _hx_array(array()));
				$i3 = 0;
				while($i3 < $l1) {
					$t1 = $b->slice($i3, $i3 + 16);
					$r1 = $r1->concat(call_user_func_array($decrypt1, array($k, $t1)));
					{
						$_g4 = 0;
						while($_g4 < 16) {
							$j2 = $_g4++;
							$r1->a[$i3 + $j2] ^= $iv1[$j2];
							unset($j2);
						}
						unset($_g4);
					}
					$iv1 = $t1->slice(0, 16);
					$i3 += 16;
				}
				return $r1;
			}break;
			case "ctr":{
				$encrypt = (isset(org_ascrypt_AES::$ie) ? org_ascrypt_AES::$ie: array("org_ascrypt_AES", "ie"));
				$iv2 = $iv->copy();
				{
					$bl = $b->length;
					$e = (new _hx_array(array()));
					$x = $iv2->copy();
					$i4 = 0;
					while($i4 < $bl) {
						$e = call_user_func_array($encrypt, array($k, $x));
						{
							$_g5 = 0;
							while($_g5 < 16) {
								$j3 = $_g5++;
								$b->a[$i4 + $j3] ^= $e[$j3];
								unset($j3);
							}
							unset($_g5);
						}
						$l2 = 15;
						while($l2 >= 0) {
							--$l2;
							$x->a[$l2]++;
							if($x[$l2] !== 0) {
								break;
							}
						}
						$i4 += 16;
						unset($l2);
					}
					return $b;
				}
			}break;
			case "none":{
				$b1 = $b->copy();
				$l3 = $k->length;
				$i5 = $l3 - 32;
				{
					$r2 = $k->slice($l3 - 16, $l3);
					{
						$_g6 = 0;
						while($_g6 < 16) {
							$i6 = $_g6++;
							$b1->a[$i6] ^= $r2[$i6];
							unset($i6);
						}
					}
				}
				{
					$t2 = org_ascrypt_AES::$isrtab;
					$h = $b1->copy();
					{
						$_g7 = 0;
						while($_g7 < 16) {
							$i7 = $_g7++;
							$b1[$i7] = $h[$t2[$i7]];
							unset($i7);
						}
					}
				}
				{
					$b2 = org_ascrypt_AES::$isbox;
					{
						$_g8 = 0;
						while($_g8 < 16) {
							$i8 = $_g8++;
							$b1[$i8] = $b2[$b1[$i8]];
							unset($i8);
						}
					}
				}
				while($i5 >= 16) {
					{
						$r3 = $k->slice($i5, $i5 + 16);
						{
							$_g9 = 0;
							while($_g9 < 16) {
								$i9 = $_g9++;
								$b1->a[$i9] ^= $r3[$i9];
								unset($i9);
							}
							unset($_g9);
						}
						unset($r3);
					}
					{
						$i10 = 0;
						while($i10 < 16) {
							$s0 = $b1[$i10];
							$s1 = $b1[$i10 + 1];
							$s2 = $b1[$i10 + 2];
							$s3 = $b1[$i10 + 3];
							$h1 = $s0 ^ $s1 ^ $s2 ^ $s3;
							$xh = org_ascrypt_AES::$xtime[$h1];
							$h11 = org_ascrypt_AES::$xtime->a[org_ascrypt_AES::$xtime[$xh ^ $s0 ^ $s2]] ^ $h1;
							$h2 = org_ascrypt_AES::$xtime->a[org_ascrypt_AES::$xtime[$xh ^ $s1 ^ $s3]] ^ $h1;
							$b1->a[$i10] ^= $h11 ^ org_ascrypt_AES::$xtime[$s0 ^ $s1];
							$b1->a[$i10 + 1] ^= $h2 ^ org_ascrypt_AES::$xtime[$s1 ^ $s2];
							$b1->a[$i10 + 2] ^= $h11 ^ org_ascrypt_AES::$xtime[$s2 ^ $s3];
							$b1->a[$i10 + 3] ^= $h2 ^ org_ascrypt_AES::$xtime[$s3 ^ $s0];
							$i10 += 4;
							unset($xh,$s3,$s2,$s1,$s0,$h2,$h11,$h1);
						}
						unset($i10);
					}
					{
						$t3 = org_ascrypt_AES::$isrtab;
						$h3 = $b1->copy();
						{
							$_g10 = 0;
							while($_g10 < 16) {
								$i11 = $_g10++;
								$b1[$i11] = $h3[$t3[$i11]];
								unset($i11);
							}
							unset($_g10);
						}
						unset($t3,$h3);
					}
					{
						$b3 = org_ascrypt_AES::$isbox;
						{
							$_g11 = 0;
							while($_g11 < 16) {
								$i12 = $_g11++;
								$b1[$i12] = $b3[$b1[$i12]];
								unset($i12);
							}
							unset($_g11);
						}
						unset($b3);
					}
					$i5 -= 16;
				}
				{
					$r4 = $k->slice(0, 16);
					{
						$_g12 = 0;
						while($_g12 < 16) {
							$i13 = $_g12++;
							$b1->a[$i13] ^= $r4[$i13];
							unset($i13);
						}
					}
				}
				return $b1;
			}break;
			default:{
				throw new HException(org_ascrypt_AES::$ERROR_MODE);
			}break;
			}
		}
	}
	static function init() {
		org_ascrypt_AES::$isrtab = new _hx_array(array());
		org_ascrypt_AES::$isbox = new _hx_array(array());
		org_ascrypt_AES::$xtime = new _hx_array(array());
		{
			$_g = 0;
			while($_g < 256) {
				$i = $_g++;
				org_ascrypt_AES::$isbox[org_ascrypt_AES::$sbox[$i]] = $i;
				unset($i);
			}
		}
		{
			$_g1 = 0;
			while($_g1 < 16) {
				$j = $_g1++;
				org_ascrypt_AES::$isrtab[org_ascrypt_AES::$srtab[$j]] = $j;
				unset($j);
			}
		}
		{
			$_g2 = 0;
			while($_g2 < 128) {
				$k = $_g2++;
				org_ascrypt_AES::$xtime[$k] = $k << 1;
				org_ascrypt_AES::$xtime[128 + $k] = $k << 1 ^ 27;
				unset($k);
			}
		}
	}
	static function sb($s, $b) {
		$_g = 0;
		while($_g < 16) {
			$i = $_g++;
			$s[$i] = $b[$s[$i]];
			unset($i);
		}
	}
	static function ark($s, $r) {
		$_g = 0;
		while($_g < 16) {
			$i = $_g++;
			$s->a[$i] ^= $r[$i];
			unset($i);
		}
	}
	static function sr($s, $t) {
		$h = $s->copy();
		{
			$_g = 0;
			while($_g < 16) {
				$i = $_g++;
				$s[$i] = $h[$t[$i]];
				unset($i);
			}
		}
	}
	static function ek($k) {
		$kl = $k->length;
		$ks = 0;
		$rcon = 1;
		switch($kl) {
		case 16:{
			$ks = 176;
		}break;
		case 24:{
			$ks = 208;
		}break;
		case 32:{
			$ks = 240;
		}break;
		}
		$i = $kl;
		while($i < $ks) {
			$t = $k->slice($i - 4, $i);
			if(_hx_mod($i, $kl) === 0) {
				$t = (new _hx_array(array(org_ascrypt_AES::$sbox->a[$t[1]] ^ $rcon, org_ascrypt_AES::$sbox[$t[2]], org_ascrypt_AES::$sbox[$t[3]], org_ascrypt_AES::$sbox[$t[0]])));
				if(($rcon <<= 1) >= 256) {
					$rcon ^= 283;
				}
			} else {
				if($kl > 24 && _hx_mod($i, $kl) === 16) {
					$t = (new _hx_array(array(org_ascrypt_AES::$sbox[$t[0]], org_ascrypt_AES::$sbox[$t[1]], org_ascrypt_AES::$sbox[$t[2]], org_ascrypt_AES::$sbox[$t[3]])));
				}
			}
			$j = 0;
			while($j < 4) {
				$k[$i + $j] = $k->a[$i + $j - $kl] ^ $t[$j];
				$j++;
			}
			$i += 4;
			unset($t,$j);
		}
	}
	static function ie($k, $ob) {
		$b = $ob->copy();
		$i = 16;
		$l = $k->length;
		{
			$r = $k->slice(0, 16);
			{
				$_g = 0;
				while($_g < 16) {
					$i1 = $_g++;
					$b->a[$i1] ^= $r[$i1];
					unset($i1);
				}
			}
		}
		while($i < $l - 16) {
			{
				$b1 = org_ascrypt_AES::$sbox;
				{
					$_g1 = 0;
					while($_g1 < 16) {
						$i2 = $_g1++;
						$b[$i2] = $b1[$b[$i2]];
						unset($i2);
					}
					unset($_g1);
				}
				unset($b1);
			}
			{
				$t = org_ascrypt_AES::$srtab;
				$h = $b->copy();
				{
					$_g2 = 0;
					while($_g2 < 16) {
						$i3 = $_g2++;
						$b[$i3] = $h[$t[$i3]];
						unset($i3);
					}
					unset($_g2);
				}
				unset($t,$h);
			}
			{
				$i4 = 0;
				while($i4 < 16) {
					$s0 = $b[$i4];
					$s1 = $b[$i4 + 1];
					$s2 = $b[$i4 + 2];
					$s3 = $b[$i4 + 3];
					$h1 = $s0 ^ $s1 ^ $s2 ^ $s3;
					$b->a[$i4] ^= $h1 ^ org_ascrypt_AES::$xtime[$s0 ^ $s1];
					$b->a[$i4 + 1] ^= $h1 ^ org_ascrypt_AES::$xtime[$s1 ^ $s2];
					$b->a[$i4 + 2] ^= $h1 ^ org_ascrypt_AES::$xtime[$s2 ^ $s3];
					$b->a[$i4 + 3] ^= $h1 ^ org_ascrypt_AES::$xtime[$s3 ^ $s0];
					$i4 += 4;
					unset($s3,$s2,$s1,$s0,$h1);
				}
				unset($i4);
			}
			{
				$r1 = $k->slice($i, $i + 16);
				{
					$_g3 = 0;
					while($_g3 < 16) {
						$i5 = $_g3++;
						$b->a[$i5] ^= $r1[$i5];
						unset($i5);
					}
					unset($_g3);
				}
				unset($r1);
			}
			$i += 16;
		}
		{
			$b2 = org_ascrypt_AES::$sbox;
			{
				$_g4 = 0;
				while($_g4 < 16) {
					$i6 = $_g4++;
					$b[$i6] = $b2[$b[$i6]];
					unset($i6);
				}
			}
		}
		{
			$t1 = org_ascrypt_AES::$srtab;
			$h2 = $b->copy();
			{
				$_g5 = 0;
				while($_g5 < 16) {
					$i7 = $_g5++;
					$b[$i7] = $h2[$t1[$i7]];
					unset($i7);
				}
			}
		}
		{
			$r2 = $k->slice($i, $i + 16);
			{
				$_g6 = 0;
				while($_g6 < 16) {
					$i8 = $_g6++;
					$b->a[$i8] ^= $r2[$i8];
					unset($i8);
				}
			}
		}
		return $b;
	}
	static function id($k, $ob) {
		$b = $ob->copy();
		$l = $k->length;
		$i = $l - 32;
		{
			$r = $k->slice($l - 16, $l);
			{
				$_g = 0;
				while($_g < 16) {
					$i1 = $_g++;
					$b->a[$i1] ^= $r[$i1];
					unset($i1);
				}
			}
		}
		{
			$t = org_ascrypt_AES::$isrtab;
			$h = $b->copy();
			{
				$_g1 = 0;
				while($_g1 < 16) {
					$i2 = $_g1++;
					$b[$i2] = $h[$t[$i2]];
					unset($i2);
				}
			}
		}
		{
			$b1 = org_ascrypt_AES::$isbox;
			{
				$_g2 = 0;
				while($_g2 < 16) {
					$i3 = $_g2++;
					$b[$i3] = $b1[$b[$i3]];
					unset($i3);
				}
			}
		}
		while($i >= 16) {
			{
				$r1 = $k->slice($i, $i + 16);
				{
					$_g3 = 0;
					while($_g3 < 16) {
						$i4 = $_g3++;
						$b->a[$i4] ^= $r1[$i4];
						unset($i4);
					}
					unset($_g3);
				}
				unset($r1);
			}
			{
				$i5 = 0;
				while($i5 < 16) {
					$s0 = $b[$i5];
					$s1 = $b[$i5 + 1];
					$s2 = $b[$i5 + 2];
					$s3 = $b[$i5 + 3];
					$h1 = $s0 ^ $s1 ^ $s2 ^ $s3;
					$xh = org_ascrypt_AES::$xtime[$h1];
					$h11 = org_ascrypt_AES::$xtime->a[org_ascrypt_AES::$xtime[$xh ^ $s0 ^ $s2]] ^ $h1;
					$h2 = org_ascrypt_AES::$xtime->a[org_ascrypt_AES::$xtime[$xh ^ $s1 ^ $s3]] ^ $h1;
					$b->a[$i5] ^= $h11 ^ org_ascrypt_AES::$xtime[$s0 ^ $s1];
					$b->a[$i5 + 1] ^= $h2 ^ org_ascrypt_AES::$xtime[$s1 ^ $s2];
					$b->a[$i5 + 2] ^= $h11 ^ org_ascrypt_AES::$xtime[$s2 ^ $s3];
					$b->a[$i5 + 3] ^= $h2 ^ org_ascrypt_AES::$xtime[$s3 ^ $s0];
					$i5 += 4;
					unset($xh,$s3,$s2,$s1,$s0,$h2,$h11,$h1);
				}
				unset($i5);
			}
			{
				$t1 = org_ascrypt_AES::$isrtab;
				$h3 = $b->copy();
				{
					$_g4 = 0;
					while($_g4 < 16) {
						$i6 = $_g4++;
						$b[$i6] = $h3[$t1[$i6]];
						unset($i6);
					}
					unset($_g4);
				}
				unset($t1,$h3);
			}
			{
				$b2 = org_ascrypt_AES::$isbox;
				{
					$_g5 = 0;
					while($_g5 < 16) {
						$i7 = $_g5++;
						$b[$i7] = $b2[$b[$i7]];
						unset($i7);
					}
					unset($_g5);
				}
				unset($b2);
			}
			$i -= 16;
		}
		{
			$r2 = $k->slice(0, 16);
			{
				$_g6 = 0;
				while($_g6 < 16) {
					$i8 = $_g6++;
					$b->a[$i8] ^= $r2[$i8];
					unset($i8);
				}
			}
		}
		return $b;
	}
	static function mc($s) {
		$i = 0;
		while($i < 16) {
			$s0 = $s[$i];
			$s1 = $s[$i + 1];
			$s2 = $s[$i + 2];
			$s3 = $s[$i + 3];
			$h = $s0 ^ $s1 ^ $s2 ^ $s3;
			$s->a[$i] ^= $h ^ org_ascrypt_AES::$xtime[$s0 ^ $s1];
			$s->a[$i + 1] ^= $h ^ org_ascrypt_AES::$xtime[$s1 ^ $s2];
			$s->a[$i + 2] ^= $h ^ org_ascrypt_AES::$xtime[$s2 ^ $s3];
			$s->a[$i + 3] ^= $h ^ org_ascrypt_AES::$xtime[$s3 ^ $s0];
			$i += 4;
			unset($s3,$s2,$s1,$s0,$h);
		}
	}
	static function mci($s) {
		$i = 0;
		while($i < 16) {
			$s0 = $s[$i];
			$s1 = $s[$i + 1];
			$s2 = $s[$i + 2];
			$s3 = $s[$i + 3];
			$h = $s0 ^ $s1 ^ $s2 ^ $s3;
			$xh = org_ascrypt_AES::$xtime[$h];
			$h1 = org_ascrypt_AES::$xtime->a[org_ascrypt_AES::$xtime[$xh ^ $s0 ^ $s2]] ^ $h;
			$h2 = org_ascrypt_AES::$xtime->a[org_ascrypt_AES::$xtime[$xh ^ $s1 ^ $s3]] ^ $h;
			$s->a[$i] ^= $h1 ^ org_ascrypt_AES::$xtime[$s0 ^ $s1];
			$s->a[$i + 1] ^= $h2 ^ org_ascrypt_AES::$xtime[$s1 ^ $s2];
			$s->a[$i + 2] ^= $h1 ^ org_ascrypt_AES::$xtime[$s2 ^ $s3];
			$s->a[$i + 3] ^= $h2 ^ org_ascrypt_AES::$xtime[$s3 ^ $s0];
			$i += 4;
			unset($xh,$s3,$s2,$s1,$s0,$h2,$h1,$h);
		}
	}
	static function check($k, $b) {
		$kl = $k->length;
		if($kl !== 16 && $kl !== 24 && $kl !== 32) {
			throw new HException(org_ascrypt_AES::$ERROR_KEY);
		}
		if(_hx_mod($b->length, 16) !== 0) {
			throw new HException(org_ascrypt_AES::$ERROR_BLOCK);
		}
	}
	function __toString() { return 'org.ascrypt.AES'; }
}
org_ascrypt_AES::$srtab = (new _hx_array(array(0, 5, 10, 15, 4, 9, 14, 3, 8, 13, 2, 7, 12, 1, 6, 11)));
org_ascrypt_AES::$sbox = (new _hx_array(array(99, 124, 119, 123, 242, 107, 111, 197, 48, 1, 103, 43, 254, 215, 171, 118, 202, 130, 201, 125, 250, 89, 71, 240, 173, 212, 162, 175, 156, 164, 114, 192, 183, 253, 147, 38, 54, 63, 247, 204, 52, 165, 229, 241, 113, 216, 49, 21, 4, 199, 35, 195, 24, 150, 5, 154, 7, 18, 128, 226, 235, 39, 178, 117, 9, 131, 44, 26, 27, 110, 90, 160, 82, 59, 214, 179, 41, 227, 47, 132, 83, 209, 0, 237, 32, 252, 177, 91, 106, 203, 190, 57, 74, 76, 88, 207, 208, 239, 170, 251, 67, 77, 51, 133, 69, 249, 2, 127, 80, 60, 159, 168, 81, 163, 64, 143, 146, 157, 56, 245, 188, 182, 218, 33, 16, 255, 243, 210, 205, 12, 19, 236, 95, 151, 68, 23, 196, 167, 126, 61, 100, 93, 25, 115, 96, 129, 79, 220, 34, 42, 144, 136, 70, 238, 184, 20, 222, 94, 11, 219, 224, 50, 58, 10, 73, 6, 36, 92, 194, 211, 172, 98, 145, 149, 228, 121, 231, 200, 55, 109, 141, 213, 78, 169, 108, 86, 244, 234, 101, 122, 174, 8, 186, 120, 37, 46, 28, 166, 180, 198, 232, 221, 116, 31, 75, 189, 139, 138, 112, 62, 181, 102, 72, 3, 246, 14, 97, 53, 87, 185, 134, 193, 29, 158, 225, 248, 152, 17, 105, 217, 142, 148, 155, 30, 135, 233, 206, 85, 40, 223, 140, 161, 137, 13, 191, 230, 66, 104, 65, 153, 45, 15, 176, 84, 187, 22)));
