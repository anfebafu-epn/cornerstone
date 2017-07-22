<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class haxe_io_Path {
	public function __construct($path) {
		if(!php_Boot::$skip_constructor) {
		switch($path) {
		case ".":case "..":{
			$this->dir = $path;
			$this->file = "";
			return;
		}break;
		}
		$c1 = _hx_last_index_of($path, "/", null);
		$c2 = _hx_last_index_of($path, "\\", null);
		if($c1 < $c2) {
			$this->dir = _hx_substr($path, 0, $c2);
			$path = _hx_substr($path, $c2 + 1, null);
			$this->backslash = true;
		} else {
			if($c2 < $c1) {
				$this->dir = _hx_substr($path, 0, $c1);
				$path = _hx_substr($path, $c1 + 1, null);
			} else {
				$this->dir = null;
			}
		}
		$cp = _hx_last_index_of($path, ".", null);
		if($cp !== -1) {
			$this->ext = _hx_substr($path, $cp + 1, null);
			$this->file = _hx_substr($path, 0, $cp);
		} else {
			$this->ext = null;
			$this->file = $path;
		}
	}}
	public $dir;
	public $file;
	public $ext;
	public $backslash;
	public function toString() {
		$tmp = null;
		if($this->dir === null) {
			$tmp = "";
		} else {
			$tmp1 = null;
			if($this->backslash) {
				$tmp1 = "\\";
			} else {
				$tmp1 = "/";
			}
			$tmp = _hx_string_or_null($this->dir) . _hx_string_or_null($tmp1);
		}
		$tmp2 = null;
		if($this->ext === null) {
			$tmp2 = "";
		} else {
			$tmp2 = "." . _hx_string_or_null($this->ext);
		}
		return _hx_string_or_null($tmp) . _hx_string_or_null($this->file) . _hx_string_or_null($tmp2);
	}
	public function __call($m, $a) {
		if(isset($this->$m) && is_callable($this->$m))
			return call_user_func_array($this->$m, $a);
		else if(isset($this->__dynamics[$m]) && is_callable($this->__dynamics[$m]))
			return call_user_func_array($this->__dynamics[$m], $a);
		else if('toString' == $m)
			return $this->__toString();
		else
			throw new HException('Unable to call <'.$m.'>');
	}
	static function withoutExtension($path) {
		$s = new haxe_io_Path($path);
		$s->ext = null;
		return $s->toString();
	}
	static function withoutDirectory($path) {
		$s = new haxe_io_Path($path);
		$s->dir = null;
		return $s->toString();
	}
	static function directory($path) {
		$s = new haxe_io_Path($path);
		if($s->dir === null) {
			return "";
		}
		return $s->dir;
	}
	static function extension($path) {
		$s = new haxe_io_Path($path);
		if($s->ext === null) {
			return "";
		}
		return $s->ext;
	}
	static function withExtension($path, $ext) {
		$s = new haxe_io_Path($path);
		$s->ext = $ext;
		return $s->toString();
	}
	static function join($paths) {
		$paths1 = $paths->filter(array(new _hx_lambda(array(), "haxe_io_Path_0"), 'execute'));
		if($paths1->length === 0) {
			return "";
		}
		$path = $paths1[0];
		{
			$_g1 = 1;
			$_g = $paths1->length;
			while($_g1 < $_g) {
				$_g1 = $_g1 + 1;
				$i = $_g1 - 1;
				$path = haxe_io_Path::addTrailingSlash($path);
				$path = _hx_string_or_null($path) . _hx_string_or_null($paths1[$i]);
				unset($i);
			}
		}
		return haxe_io_Path::normalize($path);
	}
	static function normalize($path) {
		$slash = "/";
		$path = _hx_explode("\\", $path)->join($slash);
		if($path === $slash) {
			return $slash;
		}
		$target = (new _hx_array(array()));
		{
			$_g = 0;
			$_g1 = _hx_explode($slash, $path);
			while($_g < $_g1->length) {
				$token = $_g1[$_g];
				$_g = $_g + 1;
				$tmp = null;
				$tmp1 = null;
				if($token === "..") {
					$tmp1 = $target->length > 0;
				} else {
					$tmp1 = false;
				}
				if($tmp1) {
					$tmp = $target[$target->length - 1] !== "..";
				} else {
					$tmp = false;
				}
				if($tmp) {
					$target->pop();
				} else {
					if($token !== ".") {
						$target->push($token);
					}
				}
				unset($token,$tmp1,$tmp);
			}
		}
		$tmp2 = $target->join($slash);
		$regex = new EReg("([^:])/+", "g");
		$result = $regex->replace($tmp2, "\$1" . _hx_string_or_null($slash));
		$acc = new StringBuf();
		$colon = false;
		$slashes = false;
		{
			$_g11 = 0;
			$_g2 = strlen($tmp2);
			while($_g11 < $_g2) {
				$_g11 = $_g11 + 1;
				$i = $_g11 - 1;
				{
					$_g21 = ord(substr($tmp2,$i,1));
					switch($_g21) {
					case 47:{
						if(!$colon) {
							$slashes = true;
						} else {
							$i1 = $_g21;
							{
								$colon = false;
								if($slashes) {
									$acc->add("/");
									$slashes = false;
								}
								$acc1 = $acc;
								$acc1->b = _hx_string_or_null($acc1->b) . _hx_string_or_null(chr($i1));
							}
						}
					}break;
					case 58:{
						$acc->add(":");
						$colon = true;
					}break;
					default:{
						$i2 = $_g21;
						{
							$colon = false;
							if($slashes) {
								$acc->add("/");
								$slashes = false;
							}
							$acc2 = $acc;
							$acc2->b = _hx_string_or_null($acc2->b) . _hx_string_or_null(chr($i2));
						}
					}break;
					}
					unset($_g21);
				}
				unset($i);
			}
		}
		return $acc->b;
	}
	static function addTrailingSlash($path) {
		if(strlen($path) === 0) {
			return "/";
		}
		$c1 = _hx_last_index_of($path, "/", null);
		$c2 = _hx_last_index_of($path, "\\", null);
		if($c1 < $c2) {
			if($c2 !== strlen($path) - 1) {
				return _hx_string_or_null($path) . "\\";
			} else {
				return $path;
			}
		} else {
			if($c1 !== strlen($path) - 1) {
				return _hx_string_or_null($path) . "/";
			} else {
				return $path;
			}
		}
	}
	static function removeTrailingSlashes($path) {
		while(true) {
			$_g = _hx_char_code_at($path, strlen($path) - 1);
			if($_g === null) {
				break;
			} else {
				switch($_g) {
				case 47:case 92:{
					$path = _hx_substr($path, 0, -1);
				}break;
				default:{
					break 2;
				}break;
				}
			}
			unset($_g);
		}
		return $path;
	}
	static function isAbsolute($path) {
		if(StringTools::startsWith($path, "/")) {
			return true;
		}
		if(_hx_char_at($path, 1) === ":") {
			return true;
		}
		if(StringTools::startsWith($path, "\\\\")) {
			return true;
		}
		return false;
	}
	static function unescape($path) {
		$regex = new EReg("-x([0-9][0-9])", "g");
		return $regex->map($path, array(new _hx_lambda(array(), "haxe_io_Path_1"), 'execute'));
	}
	static function escape($path, $allowSlashes = null) {
		if($allowSlashes === null) {
			$allowSlashes = false;
		}
		$regex = null;
		if($allowSlashes) {
			$regex = new EReg("[^A-Za-z0-9_/\\\\\\.]", "g");
		} else {
			$regex = new EReg("[^A-Za-z0-9_\\.]", "g");
		}
		return $regex->map($path, array(new _hx_lambda(array(), "haxe_io_Path_2"), 'execute'));
	}
	function __toString() { return $this->toString(); }
}
function haxe_io_Path_0($s) {
	{
		if($s !== null) {
			return $s !== "";
		} else {
			return false;
		}
	}
}
function haxe_io_Path_1($regex1) {
	{
		return chr(Std::parseInt($regex1->matched(1)));
	}
}
function haxe_io_Path_2($v) {
	{
		return "-x" . _hx_string_rec(_hx_char_code_at($v->matched(0), 0), "");
	}
}