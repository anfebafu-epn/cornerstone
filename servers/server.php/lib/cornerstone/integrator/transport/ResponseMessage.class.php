<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class cornerstone_integrator_transport_ResponseMessage {
	public function __construct() {
		;
	}
	public $Results;
	public $RequestSeq;
	public $Message;
	public $AttentionCode;
	public function set_Results($value) {
		return $this->Results = $value;
	}
	public function get_Results() {
		return $this->Results;
	}
	public function set_RequestSeq($value) {
		return $this->RequestSeq = $value;
	}
	public function get_RequestSeq() {
		return $this->RequestSeq;
	}
	public function set_Message($value) {
		return $this->Message = $value;
	}
	public function get_Message() {
		return $this->Message;
	}
	public function get_AttentionCode() {
		return $this->AttentionCode;
	}
	public function set_AttentionCode($value) {
		return $this->AttentionCode = $value;
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
	static $__properties__ = array("set_AttentionCode" => "set_AttentionCode","get_AttentionCode" => "get_AttentionCode","set_Message" => "set_Message","get_Message" => "get_Message","set_RequestSeq" => "set_RequestSeq","get_RequestSeq" => "get_RequestSeq","set_Results" => "set_Results","get_Results" => "get_Results");
	function __toString() { return 'cornerstone.integrator.transport.ResponseMessage'; }
}
