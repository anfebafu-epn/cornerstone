<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class cornerstone_integrator_exceptions_ExceptionManager {
	public function __construct(){}
	static function HandleException($ex, $DecryptedString = null, $req = null, $LogicModule = null, $LogicClass = null, $LogicMethod = null, $Name = null) {
		$res = new cornerstone_integrator_transport_ResponseMessage();
		$cc = Type::getClassName(Type::getClass($ex));
		if($cc === "cornerstone.integrator.exceptions.LogicException") {
			$le = _hx_cast($ex, _hx_qtype("cornerstone.integrator.exceptions.LogicException"));
			$res->set_AttentionCode("" . _hx_string_rec($le->get_Code(), ""));
			$res->set_Message($le->message);
			$res->set_RequestSeq(-1);
			$res->set_Results(null);
		} else {
			if($cc === "cornerstone.integrator.exceptions.SecurityException") {
				$le1 = _hx_cast($ex, _hx_qtype("cornerstone.integrator.exceptions.SecurityException"));
				$res->set_AttentionCode("" . _hx_string_rec($le1->get_Code(), ""));
				$res->set_Message($le1->message);
				$res->set_RequestSeq(-2);
				$res->set_Results(null);
			} else {
				$AttentionCode = Std::string(Std::random(99));
				$AttentionCode1 = _hx_string_or_null($AttentionCode) . Std::string(Std::random(99));
				$AttentionCode2 = _hx_string_or_null($AttentionCode1) . Std::string(Std::random(99));
				$AttentionCode3 = _hx_string_or_null($AttentionCode2) . Std::string(Std::random(99));
				$AttentionCode4 = _hx_string_or_null($AttentionCode3) . Std::string(Std::random(99));
				$AttentionCode5 = _hx_string_or_null($AttentionCode4) . Std::string(Std::random(99));
				$AttentionCode6 = _hx_string_or_null($AttentionCode5) . Std::string(Std::random(99));
				$AttentionCode7 = _hx_string_or_null($AttentionCode6) . Std::string(Std::random(99));
				while(true) {
					$path = _hx_string_or_null(cornerstone_integrator_configuration_Configuration::$Data->get_LogFolder()) . _hx_string_or_null($AttentionCode7) . ".txt";
					if(!file_exists($path)) {
						break;
					}
					$AttentionCode8 = Std::string(Std::random(99));
					$AttentionCode9 = _hx_string_or_null($AttentionCode8) . Std::string(Std::random(99));
					$AttentionCode10 = _hx_string_or_null($AttentionCode9) . Std::string(Std::random(99));
					$AttentionCode11 = _hx_string_or_null($AttentionCode10) . Std::string(Std::random(99));
					$AttentionCode12 = _hx_string_or_null($AttentionCode11) . Std::string(Std::random(99));
					$AttentionCode13 = _hx_string_or_null($AttentionCode12) . Std::string(Std::random(99));
					$AttentionCode14 = _hx_string_or_null($AttentionCode13) . Std::string(Std::random(99));
					$AttentionCode7 = _hx_string_or_null($AttentionCode14) . Std::string(Std::random(99));
					unset($path,$AttentionCode9,$AttentionCode8,$AttentionCode14,$AttentionCode13,$AttentionCode12,$AttentionCode11,$AttentionCode10);
				}
				$logContent = "";
				$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("Message: " . _hx_string_or_null($ex->message) . "\x0D\x0A"));
				$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("StackTrace: " . _hx_string_or_null($ex->stringStack()) . "\x0D\x0A"));
				if(_hx_field($ex, "pos") !== null) {
					$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("ClassName: " . _hx_string_or_null($ex->pos->className) . "\x0D\x0A"));
					$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("MethodName: " . _hx_string_or_null($ex->pos->methodName) . "\x0D\x0A"));
					$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("FileName: " . _hx_string_or_null($ex->pos->fileName) . "\x0D\x0A"));
					$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("LineNumber: " . _hx_string_rec($ex->pos->lineNumber, "") . "\x0D\x0A"));
				}
				$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("Date: " . _hx_string_or_null(Date::now()->toString()) . "\x0D\x0A"));
				if($DecryptedString !== null) {
					$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("SerializedData: " . _hx_string_or_null($DecryptedString) . "\x0D\x0A"));
				}
				if($req !== null) {
					$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("RequestSeq: " . _hx_string_rec($req->get_RequestSeq(), "") . "\x0D\x0A"));
					$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("SessionID: " . _hx_string_or_null($req->get_SessionID()) . "\x0D\x0A"));
					{
						$_g = 0;
						$_g1 = $req->get_Calls();
						while($_g < $_g1->length) {
							$call = $_g1[$_g];
							$_g = $_g + 1;
							$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("-------------------" . "\x0D\x0A"));
							$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("Name: " . _hx_string_or_null($call->get_Name()) . "\x0D\x0A"));
							$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("LogicModule: " . _hx_string_or_null($call->get_LogicModule()) . "\x0D\x0A"));
							$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("LogicClass: " . _hx_string_or_null($call->get_LogicClass()) . "\x0D\x0A"));
							$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("LogicMethod: " . _hx_string_or_null($call->get_LogicMethod()) . "\x0D\x0A"));
							$pcount = 1;
							{
								$_g2 = 0;
								$_g3 = $call->get_LogicParams();
								while($_g2 < $_g3->length) {
									$p = $_g3[$_g2];
									$_g2 = $_g2 + 1;
									$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("Param" . _hx_string_rec($pcount, "") . ": " . Std::string($p) . "\x0D\x0A"));
									$pcount = $pcount + 1;
									unset($p);
								}
								unset($_g3,$_g2);
							}
							unset($pcount,$call);
						}
					}
				}
				$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("-------------------" . "\x0D\x0A"));
				$logContent = _hx_string_or_null($logContent) . _hx_string_or_null(("Localhost: " . _hx_string_or_null(sys_net_Host::localhost()) . "\x0D\x0A"));
				sys_io_File::saveContent(_hx_string_or_null(cornerstone_integrator_configuration_Configuration::$Data->get_LogFolder()) . _hx_string_or_null($AttentionCode7) . ".txt", $logContent);
				$res->set_AttentionCode($AttentionCode7);
				$res->set_Message($AttentionCode7);
				$res->set_RequestSeq(-3);
				$res->set_Results(null);
			}
		}
		$serializer = new haxe_Serializer();
		$serializer->serialize($res);
		$ResponseString = $serializer->toString();
		return $ResponseString;
	}
	static function LogicException($Code, $Msg) {
		try {
			throw new HException(new cornerstone_integrator_exceptions_LogicException($Code, $Msg));
		}catch(Exception $__hx__e) {
			$_ex_ = ($__hx__e instanceof HException) && $__hx__e->getCode() == null ? $__hx__e->e : $__hx__e;
			$e = $_ex_;
			{
				throw new HException($e);
			}
		}
	}
	static function SecurityException($Code, $Msg) {
		try {
			throw new HException(new cornerstone_integrator_exceptions_SecurityException($Code, $Msg));
		}catch(Exception $__hx__e) {
			$_ex_ = ($__hx__e instanceof HException) && $__hx__e->getCode() == null ? $__hx__e->e : $__hx__e;
			$e = $_ex_;
			{
				throw new HException($e);
			}
		}
	}
	function __toString() { return 'cornerstone.integrator.exceptions.ExceptionManager'; }
}
