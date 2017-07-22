<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class haxe_ds_ObjectMap implements haxe_IMap{
	public function __construct() {
		if(!php_Boot::$skip_constructor) {
		$this->h = array();
		$this->hk = array();
	}}
	public $h;
	public $hk;
	public function set($key, $value) {
		$id = haxe_ds_ObjectMap::getId($key);
		$this->h[$id] = $value;
		$this->hk[$id] = $key;
	}
	public function get($key) {
		$id = haxe_ds_ObjectMap::getId($key);
		if(array_key_exists($id, $this->h)) {
			return $this->h[$id];
		} else {
			return null;
		}
	}
	public function exists($key) {
		return array_key_exists(haxe_ds_ObjectMap::getId($key), $this->h);
	}
	public function remove($key) {
		$id = haxe_ds_ObjectMap::getId($key);
		if(array_key_exists($id, $this->h)) {
			unset($this->h[$id]);
			unset($this->hk[$id]);
			return true;
		} else {
			return false;
		}
	}
	public function keys() {
		return new _hx_array_iterator(array_values($this->hk));
	}
	public function iterator() {
		return new _hx_array_iterator(array_values($this->h));
	}
	public function toString() {
		$s = "{";
		$it = new _hx_array_iterator(array_values($this->hk));
		{
			$i = $it;
			while($i->hasNext()) {
				$i1 = $i->next();
				$s = _hx_string_or_null($s) . Std::string($i1);
				$s = _hx_string_or_null($s) . " => ";
				$s = _hx_string_or_null($s) . Std::string($this->get($i1));
				if($it->hasNext()) {
					$s = _hx_string_or_null($s) . ", ";
				}
				unset($i1);
			}
		}
		return _hx_string_or_null($s) . "}";
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
	static function getId($key) {
		return spl_object_hash($key);
	}
	function __toString() { return $this->toString(); }
}
